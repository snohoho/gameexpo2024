using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseballLauncher : MonoBehaviour
{
    private Rigidbody rb;
    private float tossForce = 80f;

    public bool colliding;
    private bool stopped;
    private bool firing;
    private bool destroyingBalls;
    public Transform enemyTransform;

    [SerializeField] private GameObject hurtbox;
    [SerializeField] private GameObject bballPrefab;

    GameObject[] newBall = new GameObject[3];
    Rigidbody[] ballRb = new Rigidbody[3];
    Vector3[] ballRbPrevVel = new Vector3[3];
    private int ballCount = 0;
    private bool frozen;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyTransform = GetComponent<Transform>();

        stopped = false;
    }

    void FixedUpdate()
    {
        if(hurtbox.GetComponent<EnemyHitboxController>().colliding == true && !destroyingBalls) {
            StartCoroutine(RemoveBalls());
        }

        stopped = transform.parent.parent.GetComponent<ObjectManager>().stoppingTime;
        if(stopped) {
            if((newBall != null || newBall.Length != 0) && !frozen) {
                StartCoroutine(FreezeBalls());
            }
            
            rb.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        else {
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            if((newBall != null || newBall.Length != 0) && frozen) {
                StartCoroutine(UnfreezeBalls());
            }   
        }

        if(!firing && !frozen) {
            bballPrefab.SetActive(true);
            StartCoroutine(Fire());

            if(newBall[ballCount] != null) { 
                Destroy(newBall[ballCount]); 
            }

            newBall[ballCount] = Instantiate(bballPrefab, transform.parent.parent, true);
            newBall[ballCount].SetActive(true);

            ballRb[ballCount] = newBall[ballCount].GetComponent<Rigidbody>();
            ballRb[ballCount].constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ;
            ballRb[ballCount].AddForce(transform.right * tossForce + new Vector3(0,ballCount*2f,0) * 10f, ForceMode.VelocityChange);

            ballCount++;
            ballCount %= 3;
            
            bballPrefab.SetActive(false);
        }
    }

    IEnumerator Fire() {
        firing = true;

        yield return new WaitForSeconds(1);

        firing = false;
    }

    IEnumerator FreezeBalls() {
        for(int i=0; i<ballRb.Length; i++) {
            ballRbPrevVel[i] = ballRb[i].velocity;
            ballRb[i].constraints = RigidbodyConstraints.FreezeAll;

            yield return new WaitForEndOfFrame();
        }

        frozen = true;
    }

    IEnumerator UnfreezeBalls() {
        for(int i=0; i<ballRb.Length; i++) {
            ballRb[i].constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePositionZ;

            if(frozen) {
                ballRb[i].velocity = ballRbPrevVel[i];
            }  

            yield return new WaitForEndOfFrame();
        }
        
        frozen = false;
    }

    IEnumerator RemoveBalls() {
        destroyingBalls = true;

        for(int i=0; i<ballRb.Length; i++) {
            Destroy(newBall[i]);

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
        
        destroyingBalls = false;
    }
}
