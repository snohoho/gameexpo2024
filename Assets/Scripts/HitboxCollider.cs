using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxCollider : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "EnemyHurtbox" || col.gameObject.tag == "Enemy") {
            Debug.Log("hitbox to hurtbox");
            transform.parent.GetComponent<PlatformPlayer>().comboMeter++;
        }
    }
}
