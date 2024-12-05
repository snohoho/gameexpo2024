using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHurtboxController : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "PlayerHitbox" || col.gameObject.tag == "DashHitbox") {
            Debug.Log("destroying");
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
