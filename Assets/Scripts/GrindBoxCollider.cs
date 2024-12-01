using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindBoxCollider : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter(Collider col) {
        if(col.gameObject.tag == "Rail") {
            Debug.Log("grind rail contact");
            colliding = true;
            transform.parent.GetComponent<PlatformPlayer>().comboMeter++;
        }
    }

    private void OnTriggerExit(Collider col) {
        if(col.gameObject.tag == "Rail") {
            Debug.Log("grind rail exit");
            colliding = false;
        }
    }
}
