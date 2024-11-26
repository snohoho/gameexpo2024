using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindBoxCollider : MonoBehaviour
{
    public bool colliding;

    private void OnTriggerEnter(Collider col) {
        Debug.Log("grind rail contact");
        if(col.gameObject.tag == "Rail") {
            colliding = true;
        }
    }

    private void OnTriggerExit(Collider col) {
        Debug.Log("grind rail exit");
        if(col.gameObject.tag == "Rail") {
            colliding = false;
        }
    }
}
