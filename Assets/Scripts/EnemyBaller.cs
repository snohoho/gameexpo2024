using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaller : MonoBehaviour
{
    private Rigidbody rb;
    private float jumpHeight = 20f;
    private float tossForce = 20f;

    public bool colliding;
    private bool stopped;
    private bool jumping;
    private bool apex;
    public Transform enemyTransform;

    [SerializeField] private GameObject hurtbox;
    [SerializeField] private GameObject bballPrefab;

    GameObject newBall;
    Rigidbody ballRb;
    Vector3 ballRbPrevVel;
    private bool frozen;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyTransform = GetComponent<Transform>();

        stopped = false;
    }

    void FixedUpdate()
    {
        if(transform.parent.parent.GetComponent<ObjectManager>().gamePaused) {
            return;
        }

        if(hurtbox.GetComponent<EnemyHitboxController>().colliding == true) {
            if(newBall != null) {
                Destroy(newBall);
            }   
            Destroy(gameObject);
        }

        stopped = transform.parent.parent.GetComponent<ObjectManager>().stoppingTime;
        if(stopped) {
            if(newBall != null && !frozen) {
                ballRbPrevVel = ballRb.velocity;
                ballRb.constraints = RigidbodyConstraints.FreezeAll;
                frozen = true;
            }
            
            rb.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        else {
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            if(newBall != null) {
                ballRb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ;

                if(frozen) {
                    frozen = false;
                    ballRb.velocity = ballRbPrevVel;
                }   
            }   
        }

        if(!jumping) {
            apex = false;
            bballPrefab.SetActive(true);
            StartCoroutine(Jump());
        }

        RaycastHit ray;
        if(!Physics.Raycast(transform.position, -transform.up, out ray, 1f) && rb.velocity.y <= 0 && apex == false) {
            Debug.Log("jump apex reached");
            bballPrefab.SetActive(false);
            apex = true;

            if(newBall != null) { 
                Destroy(newBall); 
            }

            newBall = Instantiate(bballPrefab, transform.parent.parent,true);
            newBall.SetActive(true);

            ballRb = newBall.GetComponent<Rigidbody>();
            ballRb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ;
            ballRb.AddForce(transform.right * tossForce + new Vector3(0,2.5f,0) * tossForce, ForceMode.VelocityChange);    
        }
    }

    IEnumerator Jump() {
        jumping = true;
        rb.AddForce(transform.up * jumpHeight, ForceMode.VelocityChange);

        yield return new WaitForSeconds(3);

        jumping = false;
    }
}
