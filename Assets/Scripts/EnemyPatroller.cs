using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroller : MonoBehaviour
{
    private Rigidbody rb;
    private float speed = 5f;
    private bool lookingLeft;

    public bool colliding;
    private bool stopped;
    public Transform enemyTransform;

    [SerializeField] private GameObject hurtbox;

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        enemyTransform = GetComponent<Transform>();

        lookingLeft = true;
        stopped = false;
    }

    void FixedUpdate() 
    {
        if(hurtbox.GetComponent<EnemyHitboxController>().colliding == true) {
            Destroy(gameObject);
        }

        stopped = transform.parent.parent.GetComponent<ObjectManager>().stoppingTime;
        if(stopped) {
            rb.velocity = Vector3.zero;
            return;
        }

        if(lookingLeft) {
            transform.localRotation = Quaternion.Euler(0,0,0); 
        }
        else {
            transform.localRotation = Quaternion.Euler(0,-180,0); 
        }
        
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "PatrolPt") {
            lookingLeft = !lookingLeft;
        }
    }
}
