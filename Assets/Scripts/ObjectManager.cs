using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject player; 
    public bool stoppingTime;
    public int hp;
    public float timeStopBar;
    public int comboMeter;

    void Awake() {
        hp = player.GetComponent<PlatformPlayer>().hp;
        timeStopBar = player.GetComponent<PlatformPlayer>().timeStopBar;
        comboMeter = player.GetComponent<PlatformPlayer>().comboMeter;
    }

    void Update()
    {
        stoppingTime = player.GetComponent<PlatformPlayer>().stoppingTime;
        hp = player.GetComponent<PlatformPlayer>().hp;
        timeStopBar = player.GetComponent<PlatformPlayer>().timeStopBar;
        comboMeter = player.GetComponent<PlatformPlayer>().comboMeter;
    }
}
