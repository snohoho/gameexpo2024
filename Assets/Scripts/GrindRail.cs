using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class GrindRail : MonoBehaviour
{
    public SplineContainer spline;
    public float length;

    void Start()
    {
        spline = GetComponent<SplineContainer>();
        length = spline.CalculateLength();
    }
}
