using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Splines;
using TMPro;

public class PlatformPlayer : MonoBehaviour
{
    //movement
    private Rigidbody rb;
    private Vector2 moveInput;
    private bool lookingLeft;

    //speed params
    [Header("Speed Params")]
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float acceleration = 65f;

    //jump params
    [Header("Jump Params")]
    [SerializeField] private float jumpHeight = 25f;
    private bool canJump;
    private bool jumping;

    //dash params
    [Header("Dash Params")]
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dropDashSpeed = 50f;
    [SerializeField] private int dashCount = 2;
    private bool dashing;
    private int dashTimer = 0;
    private int dropDashTimer = 0;
    private bool dropDashing;

    //grinding params
    [Header("Grinding")]
    //[SerializeField] private Collider grindBox;
    private bool grinding;
    private GrindRail currentRail;
    float timeForFullSpline;
    float elapsedTime;

    //debug
    [Header("Debugging")]
    [SerializeField] private TextMeshPro speedTrack;
    [SerializeField] private TextMeshPro lastDirTrack;
    [SerializeField] private TextMeshPro ddTimerTrack;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            throw new System.Exception("Object doesn't have rigidbody");
        }

        canJump = true;
        lookingLeft = true;
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3(moveInput.x, 0, 0).normalized * acceleration, ForceMode.Acceleration);
        var velX = rb.velocity.x;
        var velY = rb.velocity.y;

        //max speed control
        if(velX > maxSpeed) {
            velX = maxSpeed;
        }
        if(velX < -maxSpeed) {
            velX = -maxSpeed;
        }
        if(velY > jumpHeight) {
            velY = jumpHeight;
        }

        //dir control
        if(moveInput.x > 0) {
            transform.localRotation = Quaternion.Euler(0,0,0);
            lookingLeft = true;
        }
        if(moveInput.x < 0) {
            transform.localRotation  = Quaternion.Euler(0,-180,0);
            lookingLeft = false;
        }
        
        rb.velocity = new Vector3(velX, velY, 0);

        //jump handling
        if(jumping && canJump) {
            canJump = false;
            rb.AddForce(new Vector3(0, 1, 0).normalized * jumpHeight, ForceMode.VelocityChange);
        }

        //dash handling
        if(dashing && dashCount >= 0) {
            dashing = false;
            rb.velocity = Vector3.zero;

            //dash if not holding a mov direction
            if(Mathf.Round(moveInput.x) == 0 && Mathf.Round(moveInput.y) == 0) {
                if(Mathf.Round(transform.localRotation.y) == 0) {
                    rb.AddForce(new Vector3(1,0,0) * dropDashSpeed, ForceMode.VelocityChange); 
                }
                else {
                    rb.AddForce(new Vector3(-1,0,0) * dropDashSpeed, ForceMode.VelocityChange); 
                }
            }
            else {
                rb.AddForce(new Vector3(moveInput.x, moveInput.y, 0).normalized * dashSpeed, ForceMode.VelocityChange);
            }

            if(moveInput.y < 0 && Mathf.Round(moveInput.x) == 0) {
                dropDashing = true;
            }
            else {
                dropDashing = false;
            }

            rb.useGravity = false;

            lastDirTrack.text = moveInput.x.ToString() + " " + moveInput.y.ToString() + " " + moveInput.magnitude;   
        }

        dashTimer += 1;
        dropDashTimer += 1;

        if(dashTimer >= 10) {
            rb.useGravity = true;
        }

        //grind handling
        if(grinding) {
            rb.velocity = Vector3.zero;
            float progress = elapsedTime / timeForFullSpline;

            if(progress < 0 || progress > 1) {
                grinding = false;
                transform.position += transform.right * 1;
                return;
            }

            float3 railPos = currentRail.spline.Spline.EvaluatePosition(progress);
            railPos = currentRail.transform.TransformPoint(railPos);
            transform.position = railPos.toVector3() + transform.up+1;

            if(lookingLeft) {
                elapsedTime -= Time.fixedDeltaTime;
            }
            else {
                elapsedTime += Time.fixedDeltaTime;
            }
        }

        speedTrack.text = rb.velocity.magnitude.ToString();  
        ddTimerTrack.text = dropDashTimer.ToString();
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ground") {
            Debug.Log("floor contact");
            canJump = true;
            dashCount = 2;
            rb.useGravity = true;
            dashTimer = 0;

            //drop dash
            if(moveInput.y < -0.7 && dropDashTimer <= 30 || dropDashing) {
                Debug.Log("BOOM DROP DASH");
                if(lookingLeft) {
                    rb.AddForce(new Vector3(1,0,0) * dropDashSpeed, ForceMode.VelocityChange); 
                }
                else {
                    rb.AddForce(new Vector3(-1,0,0) * dropDashSpeed, ForceMode.VelocityChange); 
                }
            }

            dropDashing = false;
        }    
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Rail") {
            Debug.Log("grind rail contact");

            currentRail = col.gameObject.GetComponent<GrindRail>();

            float3 nearestPoint;
            Vector3 playerRailStart = currentRail.transform.InverseTransformPoint(transform.position);
            SplineUtility.GetNearestPoint(currentRail.spline.Spline, playerRailStart, out nearestPoint, out float time);

            nearestPoint = currentRail.transform.TransformPoint(nearestPoint);
            transform.position = nearestPoint.toVector3();

            timeForFullSpline = currentRail.length / rb.velocity.magnitude;
            elapsedTime = timeForFullSpline * time;

            grinding = true;
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Rail") {
            Debug.Log("grind rail exit");
            grinding = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) {
            moveInput = context.ReadValue<Vector2>();
            
        }
        if (context.canceled) {
            moveInput = Vector2.zero;
        }
    }

    public void Jump(InputAction.CallbackContext context) 
    {
        if(context.started) {
            jumping = true;
        }
        if(context.canceled) {
            jumping = false;
        }
    }

    public void Dash(InputAction.CallbackContext context) 
    {
        if(context.started) {
            dashing = true;
            dropDashing = false;
            dashCount--;
            dashTimer = 0;
            dropDashTimer = 0;
        }
        if(context.canceled) {
            dashing = false;
        }
    }
}
