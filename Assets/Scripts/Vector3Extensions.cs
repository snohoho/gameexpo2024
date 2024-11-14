using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public static class Vector3Extensions
{
    public static Vector3 toVector3(this float3 floatPos) {
        return new Vector3(floatPos.x, floatPos.y, floatPos.z);
    } 
}
