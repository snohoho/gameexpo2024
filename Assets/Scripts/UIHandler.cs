using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIHandler : MonoBehaviour
{
    [SerializeField] private GameObject timeStopSlider;
    private Slider slider;

    [SerializeField] private GameObject comboMeter;
    private TextMeshProUGUI comboMeterText;

    [SerializeField] private GameObject hpPanel;
    private GameObject[] hpUi = new GameObject[3];
    private int activeCount = 3;

    [SerializeField] private GameObject timer;
    private TextMeshProUGUI timerText; 
    private float time;

    void Start()
    {
        slider = timeStopSlider.GetComponent<Slider>(); 
        slider.maxValue = transform.parent.GetComponent<ObjectManager>().timeStopBar;

        comboMeterText = comboMeter.GetComponent<TextMeshProUGUI>();

        hpUi[0] = hpPanel.transform.GetChild(0).gameObject;
        hpUi[1] = hpPanel.transform.GetChild(1).gameObject;
        hpUi[2] = hpPanel.transform.GetChild(2).gameObject; 

        timerText = timer.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        slider.value = transform.parent.GetComponent<ObjectManager>().timeStopBar;
        if(transform.parent.GetComponent<ObjectManager>().comboMeter > 0 ) {
            comboMeter.transform.parent.gameObject.SetActive(true);
            comboMeterText.text = transform.parent.GetComponent<ObjectManager>().comboMeter.ToString();
        }
        else if(transform.parent.GetComponent<ObjectManager>().comboMeter <= 0) {
            comboMeter.transform.parent.gameObject.SetActive(false);
        }

        int hp = transform.parent.GetComponent<ObjectManager>().hp;
        if(hp < 3) {
            if(!hpUi[hp-1].activeSelf && hp > activeCount) {
                hpUi[hp-1].SetActive(true);
                activeCount++;
            }
            else if(hpUi[hp].activeSelf) {
                hpUi[hp].SetActive(false);
                activeCount--;
            }
        }
        else if(!hpUi[hp-1].activeSelf && hp == 3) {
            hpUi[hp-1].SetActive(true);
        }

        time = Time.timeSinceLevelLoad;
        int ms = ((int)(time*1000f)%1000);
        int ss = (int)time%60;
        int mm = ((int)time/60)%60;

        string msString = ms.ToString();

        if(msString.Length > 2) { msString = msString.Substring(0,2); }
        timerText.text = string.Format("{0:00}:{1:00}.{2:00}",mm,ss,msString);
    }
}
