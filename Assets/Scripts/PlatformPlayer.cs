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
    private float currentSpeed;

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
    [SerializeField] private GameObject grindBox;
    private bool grinding;
    private GrindRail currentRail;
    float timeForFullSpline;
    float elapsedTime;
    private Vector2 grindMoveInputStorage;
    private float grindOffset = 1;

    //tricking params
    [Header("Tricking + Combos")]
    [SerializeField] private int trickCooldown = 30;
    private int trickTimer = 0;
    private int manualTimer = 0;
    private bool tricking;
    private bool manualing;
    private int comboMeter;

    //debug
    [Header("Debugging")]
    [SerializeField] private TextMeshPro speedTrack;
    [SerializeField] private TextMeshPro lastDirTrack;
    [SerializeField] private TextMeshPro ddTimerTrack;
    [SerializeField] private TextMeshPro trickTimerTrack;
    [SerializeField] private TextMeshPro comboMeterTrack;
    [SerializeField] private TextMeshPro manualTrack;
    [SerializeField] private TextMeshPro manualTimerTrack;

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
                rb.AddForce(transform.right * dashSpeed, ForceMode.VelocityChange); 
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

        dashTimer++;
        dropDashTimer++;

        if(dashTimer >= 10) {
            rb.useGravity = true;
        }

        //grind handling
        if(grinding) {
            rb.velocity = Vector3.zero;
            float progress = elapsedTime / timeForFullSpline;

            if(progress < 0 || progress > 1 || jumping) {
                grinding = false;
                if(jumping) {
                    if(grindMoveInputStorage.x > 0) {
                        transform.localRotation = Quaternion.Euler(0,0,0);
                        lookingLeft = true;
                    }
                    if(grindMoveInputStorage.x < 0) {
                        transform.localRotation  = Quaternion.Euler(0,-180,0);
                        lookingLeft = false;
                    }
                    
                    rb.AddForce(new Vector3(grindMoveInputStorage.x, 1, 0).normalized * jumpHeight, ForceMode.VelocityChange);
                }
                rb.AddForce(transform.right * currentSpeed, ForceMode.VelocityChange);;
                return;
            }

            float3 railPos = currentRail.spline.Spline.EvaluatePosition(progress);
            railPos = currentRail.transform.TransformPoint(railPos);
            transform.position = railPos.toVector3() + transform.up*grindOffset;
            //transform.up += new Vector3(0,grindOffset,0);

            if(lookingLeft) {
                elapsedTime -= Time.fixedDeltaTime;
            }
            else {
                elapsedTime += Time.fixedDeltaTime;
            }
        }

        //trick handling
        if(tricking) {
            comboMeter++;
            tricking = false;
        }
        if(!manualing) {
            manualTimer--;
            if(manualTimer <= 0) {
                RaycastHit ray;
                if(Physics.Raycast(transform.position, -transform.up, out ray, 1f)) {
                    //Debug.Log("not manualing while on ground end combo");
                    comboMeter = 0;
                }
                manualing = false;
            }
        }
        else {
            manualTimer = 20;
        }

        trickTimer--;

        //debug
        speedTrack.text = rb.velocity.magnitude.ToString();  
        ddTimerTrack.text = dropDashTimer.ToString();
        trickTimerTrack.text = trickTimer.ToString();
        comboMeterTrack.text = comboMeter.ToString();
        manualTrack.text = manualing.ToString();
        manualTimerTrack.text = manualTimer.ToString();
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ground") {
            Debug.Log("floor contact");
            canJump = true;
            dashCount = 2;
            if(!manualing) {
                comboMeter = 0;
            }
            
            rb.useGravity = true;
            dashTimer = 0;

            //drop dash
            if(moveInput.y < -0.7 && dropDashTimer <= 30 || dropDashing) {
                Debug.Log("BOOM DROP DASH");
                rb.AddForce(transform.right * dropDashSpeed, ForceMode.VelocityChange); 
            }

            dropDashing = false;
        }    
    }

    private void OnTriggerEnter(Collider col) {
        if(grindBox.gameObject.GetComponent<GrindBoxCollider>().colliding) {
            //Debug.Log("grind rail contact");

            currentRail = col.gameObject.GetComponent<GrindRail>();
            currentSpeed = rb.velocity.magnitude;

            float3 nearestPoint;
            Vector3 playerRailStart = currentRail.transform.InverseTransformPoint(transform.position);
            SplineUtility.GetNearestPoint(currentRail.spline.Spline, playerRailStart, out nearestPoint, out float time);

            nearestPoint = currentRail.transform.TransformPoint(nearestPoint);
            transform.position = nearestPoint.toVector3();

            timeForFullSpline = currentRail.length / currentSpeed;
            elapsedTime = timeForFullSpline * time;

            dropDashing = false;
            jumping = false;
            grinding = true;
        }
    }

    private void OnTriggerExit(Collider col) {
        if(!grindBox.gameObject.GetComponent<GrindBoxCollider>().colliding) {
            //Debug.Log("grind rail exit");
            grinding = false;
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started || context.performed) {
            if(grinding) {
                grindMoveInputStorage = context.ReadValue<Vector2>();
                return;
            }
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
            dashTimer = 0;
            dropDashTimer = 0;

            //cast a ray down. if it doesnt hit the ground then update the dash counter
            //also updates if they dash up
            RaycastHit ray;
            if(!Physics.Raycast(transform.position, -transform.up, out ray, 1f) || moveInput.y > 0) {
                Debug.Log("update dash");
                dashCount--;
            }
        }
        if(context.canceled) {
            dashing = false;
        }
    }

    public void Trick(InputAction.CallbackContext context) {
        if(context.started && trickTimer <= 0) {
            trickTimer = trickCooldown;
            tricking = true;

            //cast a ray down. if it hits the ground then enter a manual
            RaycastHit ray;
            if(Physics.Raycast(transform.position, -transform.up, out ray, 1f)) {
                Debug.Log("maunal");
                manualing = true;
            }
        }
        if(context.performed) {
            manualing = true;
        }
        if(context.canceled) {
            tricking = false;
            manualing = false; 
        }
    }
}
