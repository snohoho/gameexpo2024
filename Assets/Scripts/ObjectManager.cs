using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] GameObject player; 
    public bool stoppingTime;

    void Update()
    {
        stoppingTime = player.GetComponent<PlatformPlayer>().stoppingTime;
    }
}
