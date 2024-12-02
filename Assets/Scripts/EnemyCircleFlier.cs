using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCircleFlier : MonoBehaviour
{
    private Rigidbody rb;
    private bool lookingLeft;
    private float timeSpentFrozen = 0f;
    private bool frozen;

    [SerializeField] private float xSpeed = 1f;
    [SerializeField] private float ySpeed = 1f;
    [SerializeField] private float xRad = 5f;
    [SerializeField] private float yRad = 5f;
    [SerializeField] private float xOffset = 0f;
    [SerializeField] private float yOffset = 0f;

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
        if(transform.parent.parent.GetComponent<ObjectManager>().gamePaused) {
            return;
        }
        
        if(hurtbox.GetComponent<EnemyHitboxController>().colliding == true) {
            Destroy(gameObject);
        }

        stopped = transform.parent.parent.GetComponent<ObjectManager>().stoppingTime;
        if(stopped) {
            timeSpentFrozen += Time.fixedDeltaTime;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            return;
        }
        else {
            rb.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        float x = Mathf.Cos((Time.time - timeSpentFrozen + xOffset) * xSpeed) * xRad;
        float y = Mathf.Sin((Time.time - timeSpentFrozen + yOffset) * ySpeed) * yRad;
        transform.localPosition = new Vector3(x,y,0);
    }
}
