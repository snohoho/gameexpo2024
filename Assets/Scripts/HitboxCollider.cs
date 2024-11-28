using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollider : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "EnemyHurtbox" && !transform.parent.GetComponent<PlatformPlayer>().invuln) {
            Debug.Log("hitbox to hurtbox");
            transform.parent.GetComponent<PlatformPlayer>().comboMeter++;

            var rb = transform.parent.GetComponent<Rigidbody>();
            rb.AddForce(transform.up * 50f, ForceMode.VelocityChange); 
        }
    }
}
