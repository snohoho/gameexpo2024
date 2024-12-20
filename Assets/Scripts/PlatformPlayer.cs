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
    public Vector2 moveInput;
    public PlayerAnimationHandler animHandler;
    private bool lookingRight;
    public float currentSpeed;
    public bool invuln;
    private bool dead;
    public bool levelComplete;
    public bool Dead
    {
        get => dead;
        set
        {
            dead = value;
            animHandler.animator.SetBool("dead", dead);
        }
    }


    //hp handling
    public int hp = 3;

    //speed params
    [Header("Speed Params")]
    [SerializeField] private float maxSpeed = 25f;
    [SerializeField] private float acceleration = 65f;

    //jump params
    [Header("Jump Params")]
    [SerializeField] private float jumpHeight = 25f;
    public bool canJump;
    public bool jumping;

    //dash params
    [Header("Dash Params")]
    [SerializeField] public GameObject dashHitbox;
    [SerializeField] private float dashSpeed = 50f;
    [SerializeField] private float dropDashSpeed = 50f;
    [SerializeField] public int dashCount = 2;
    public bool dashing;
    private float dashTimer = 0;
    private float dropDashTimer = 0;
    private bool dropDashing;

    //grinding params
    [Header("Grinding")]
    [SerializeField] private GameObject grindBox;
    public bool grinding;
    private GrindRail currentRail;
    private float timeForFullSpline;
    private float elapsedTime;
    private Vector2 grindMoveInputStorage;
    private float grindOffset = 1.825f;

    //tricking params
    [Header("Tricking + Combos")]
    [SerializeField] private float trickCooldown = 30;
    private float trickTimer = 0;
    private float manualTimer = 0;
    public bool tricking;
    public bool manualing;
    public int comboMeter;
    public int maxCombo;

    //timestop params
    [Header("Time Stop")]
    [SerializeField] private float timeStopBarMax = 180;
    public float timeStopBar = 180;
    private bool stoppingTime;
    public bool StoppingTime{
        get => stoppingTime;
        set
        {
            stoppingTime = value;
            if (stoppingTime)
            {
                animHandler.animator.SetTrigger("stoppingTime2");
            }
            
        }
    }
    
    //other
    [Header("Other Stuff")]
    [SerializeField] private Canvas inv;
    public AudioHandler audioHandler;
    public PlayerSoundHolder fx;

    //debug
    [Header("Debugging")]
    [SerializeField] private TextMeshPro speedTrack;
    [SerializeField] private TextMeshPro lastDirTrack;
    [SerializeField] private TextMeshPro ddTimerTrack;
    [SerializeField] private TextMeshPro trickTimerTrack;
    [SerializeField] private TextMeshPro comboMeterTrack;
    [SerializeField] private TextMeshPro manualTrack;
    [SerializeField] private TextMeshPro manualTimerTrack;
    [SerializeField] private TextMeshPro timeStopBarTrack;

    void Awake() {
        timeStopBarMax *= Time.fixedDeltaTime;
        trickCooldown *= Time.fixedDeltaTime;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            throw new System.Exception("Object doesn't have rigidbody");
        }
        animHandler = GetComponent<PlayerAnimationHandler>();
        fx = GetComponent<PlayerSoundHolder>();

        canJump = true;
        lookingRight = true;
        StoppingTime = false;
    }

    void FixedUpdate()
    {
        if(transform.parent.GetComponent<ObjectManager>().gamePaused) {
            return;
        }

        if(hp <= 0) {
            Dead = true;
            return;
        }

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

        if(!dashing && !grinding) {
            currentSpeed = rb.velocity.x;
        }

        //dir control
        if(moveInput.x > 0) {
            transform.localRotation = Quaternion.Euler(0,0,0);
            lookingRight = true;
            animHandler.model.transform.localScale = new Vector3(1,1,1)/2;
        }
        if(moveInput.x < 0) {
            transform.localRotation  = Quaternion.Euler(0,-180,0);
            lookingRight = false;
            animHandler.model.transform.localScale = new Vector3(1,1,-1)/2;
        }
        
        rb.velocity = new Vector3(velX, velY, 0);

        //jump handling
        if(jumping && canJump) {
            canJump = false;
            rb.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);
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

        dashTimer += Time.fixedDeltaTime;
        dropDashTimer += Time.fixedDeltaTime;

        if(dashTimer >= 10 * Time.fixedDeltaTime) {
            dashHitbox.SetActive(false);
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
                        lookingRight = true;
                        animHandler.model.transform.localScale = new Vector3(1,1,1)/2;
                    }
                    if(grindMoveInputStorage.x < 0) {
                        transform.localRotation  = Quaternion.Euler(0,-180,0);
                        lookingRight = false;
                        animHandler.model.transform.localScale = new Vector3(1,1,-1)/2;
                    }
                    
                    rb.AddForce(new Vector3(grindMoveInputStorage.x, 1, 0).normalized * jumpHeight, ForceMode.VelocityChange);
                }
                rb.AddForce(transform.right * currentSpeed, ForceMode.VelocityChange);
                moveInput = grindMoveInputStorage;
                return;
            }

            float3 railPos = currentRail.spline.Spline.EvaluatePosition(progress);
            railPos = currentRail.transform.TransformPoint(railPos);
            transform.position = railPos.toVector3() + transform.up*grindOffset;
            //transform.up += new Vector3(0,grindOffset,0);

            if(lookingRight) {
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
            if(grinding) {
                audioHandler.PlayClip(fx.grindTrickFx);
            }
        }

        if(!manualing || (Mathf.Abs(rb.velocity.x) < 0.5f && !grinding)) {
            manualTimer -= Time.fixedDeltaTime;
            if(manualTimer <= 0) {
                RaycastHit ray;
                if(Physics.Raycast(transform.position, -transform.up, out ray, 2f) && ray.collider.tag == "Ground") {
                    comboMeter = 0;
                }
                manualing = false;
            }
        }
        else {
            manualTimer = 20 * Time.fixedDeltaTime;
        }

        trickTimer -= Time.fixedDeltaTime;

        if(comboMeter > maxCombo) {
            maxCombo = comboMeter;
        }
        
        //timestop handling
        if(timeStopBar >= timeStopBarMax) {
            timeStopBar = timeStopBarMax;
        }

        if(StoppingTime) {
            timeStopBar -= Time.fixedDeltaTime;
        }
        else if(!StoppingTime) {
            timeStopBar += Time.fixedDeltaTime/3;
        }
        if(StoppingTime && timeStopBar <= 0) {
            StoppingTime = false;
        }
        
        //debug
        speedTrack.text = rb.velocity.magnitude.ToString();  
        ddTimerTrack.text = dropDashTimer.ToString();
        trickTimerTrack.text = trickTimer.ToString();
        comboMeterTrack.text = comboMeter.ToString();
        manualTrack.text = manualing.ToString();
        manualTimerTrack.text = manualTimer.ToString();
        timeStopBarTrack.text = timeStopBar.ToString();
    }

    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Ground") {
            Debug.Log("floor contact");
            canJump = true;
            dashCount = 2;
            if(!manualing && !grinding) {
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
            
            audioHandler.PlayClip(fx.landFx);
        }
        if(col.gameObject.tag == "Enemy" && !invuln) {
            hp--;
            comboMeter = 0;
            grinding = false;
            rb.AddForce(-transform.right * 50f + transform.up * 20f, ForceMode.VelocityChange);

            audioHandler.PlayClip(fx.damageFx);
            if(hp > 0) {
                StartCoroutine(InvulnFrames());
            }
        }
    }

    private void OnTriggerEnter(Collider col) {
        if(grindBox.gameObject.GetComponent<GrindBoxCollider>().colliding) {
            //Debug.Log("grind rail contact");

            currentRail = col.gameObject.GetComponent<GrindRail>();
            currentSpeed = rb.velocity.magnitude;
            if(currentSpeed > 25.0f) {
                currentSpeed = 25.0f;
            }

            float3 nearestPoint;
            Vector3 playerRailStart = currentRail.transform.InverseTransformPoint(transform.position);
            SplineUtility.GetNearestPoint(currentRail.spline.Spline, playerRailStart, out nearestPoint, out float time);

            nearestPoint = currentRail.transform.TransformPoint(nearestPoint);
            transform.position = nearestPoint.toVector3();

            timeForFullSpline = currentRail.length / currentSpeed;
            elapsedTime = timeForFullSpline * time;

            grindMoveInputStorage = transform.right;

            dropDashing = false;
            jumping = false;
            grinding = true;
            dashCount = 2;

            audioHandler.PlayClip(fx.grindingFx);
        }

        if(col.gameObject.tag == "EndPoint") {
            levelComplete = true;
            audioHandler.PlayClip(fx.levelWinFx);
        }
    }

    private void OnTriggerExit(Collider col) {
        if(!grindBox.gameObject.GetComponent<GrindBoxCollider>().colliding && grinding) {
            Debug.Log("testtt23");
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

            if(canJump) {
                audioHandler.PlayClip(fx.jumpFx);
            }
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
            dashHitbox.SetActive(true);

            //cast a ray down. if it doesnt hit the ground then update the dash counter
            //also updates if they dash up
            RaycastHit ray;
            if(!Physics.Raycast(transform.position, -transform.up, out ray, 2f) || moveInput.y > 0) {
                Debug.Log("update dash");
                dashCount--;
            }

            if(dashCount > 0) {
                audioHandler.PlayClip(fx.dashFx);
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
            if(Physics.Raycast(transform.position, -transform.up, out ray, 2f)) {
                Debug.Log("maunal");
                manualing = true;
            }

            if(trickTimer <= 0) {
                audioHandler.PlayClip(fx.trickFx);
            }
        }
        if(context.performed) {
            Debug.Log("hold manual");
            manualing = true;
        }
        if(context.canceled) {
            tricking = false;
            manualing = false; 
        }
    }

    public void TimeStop(InputAction.CallbackContext context) {
        if(context.started && timeStopBar > 0) {
            StoppingTime = !StoppingTime;

            if(StoppingTime) {
                audioHandler.PlayClip(fx.stoppingTimeFx);
            }     
        }
    }

    public void EffectHandler(string powerUp) {
        audioHandler.PlayClip(fx.powerupUseFx);

        switch(powerUp) {
            case "JumpPu":
                rb.AddForce(transform.up * 200f, ForceMode.VelocityChange);
                
                break;
            case "DashPu":
                dashCount += 1;

                break;
            case "TimePu":
                timeStopBar = timeStopBarMax;

                break;
            case "HealthPu":
                if(hp < 3) {
                    hp++;
                }
                
                break;
            default:
                break;
        }
    }

    IEnumerator InvulnFrames() {
        invuln = true;
        int count = 0;
        GameObject model = animHandler.model;

        while(count < 5) {
            animHandler.model.SetActive(false);

            yield return new WaitForSeconds(0.1f);

            animHandler.model.SetActive(true);

            yield return new WaitForSeconds(0.1f);
            
            count++;
        }

        animHandler.model.SetActive(true);
        invuln = false;
    }
}
