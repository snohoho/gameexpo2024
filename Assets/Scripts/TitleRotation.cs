using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleRotation : MonoBehaviour
{
    private float rot;
    private float rotSpd = 0.25f;
    private float minRot = -2.5f;
    private float maxRot = 2.5f;

    void Update()
    {
        float rot = Mathf.SmoothStep(minRot, maxRot, Mathf.PingPong(Time.time * rotSpd, 1));
		transform.rotation = Quaternion.Euler(0,0,rot);
    }
}
