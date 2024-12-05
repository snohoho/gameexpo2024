using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollider : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "EnemyHurtbox" && !transform.parent.GetComponent<PlatformPlayer>().invuln) {
            Debug.Log("hitbox to hurtbox");
            StartCoroutine(OnHitInvuln());
            transform.parent.GetComponent<PlatformPlayer>().comboMeter++;

            var rb = transform.parent.GetComponent<Rigidbody>();
            if(!transform.parent.GetComponent<PlatformPlayer>().dashHitbox.activeSelf) {
                rb.AddForce(transform.parent.up * 50f, ForceMode.VelocityChange); 
            }
        }
    }

    IEnumerator OnHitInvuln() {
        transform.parent.GetComponent<PlatformPlayer>().invuln = true;
        yield return new WaitForSeconds(0.05f);
        transform.parent.GetComponent<PlatformPlayer>().invuln = false;
    }
}
