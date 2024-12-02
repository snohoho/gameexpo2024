using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject player; 
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Animator pauseAnimator;
    [SerializeField] private Transform startPos;
    [SerializeField] private GameObject transition;
    [SerializeField] private GameObject screenUI;
    [SerializeField] private GameObject finalStats;
    public bool stoppingTime;
    public int hp;
    public float timeStopBar;
    public int comboMeter;
    public bool gamePaused;
    public bool blah;

    void Awake() {
        hp = player.GetComponent<PlatformPlayer>().hp;
        timeStopBar = player.GetComponent<PlatformPlayer>().timeStopBar;
        comboMeter = player.GetComponent<PlatformPlayer>().comboMeter;

        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    void Start() {
        gamePaused = false;
        blah = false;
        pauseMenu.SetActive(false);
        finalStats.SetActive(false);
    }

    void Update()
    {
        if(player.GetComponent<PlatformPlayer>().levelComplete) {
            finalStats.SetActive(true);
            Time.timeScale = 0f;
            var statText = finalStats.transform.GetChild(0).GetChild(0);
            statText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "final time:\n" + screenUI.GetComponent<UIHandler>().timerText.text;
            statText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "max combo:\n" + player.GetComponent<PlatformPlayer>().maxCombo.ToString();

            if(!blah) {
                blah = true;
                pauseAnimator.SetTrigger("openPause");
            }
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if(gamePaused) {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;

            pauseAnimator.SetTrigger("openPause");
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        if(player.GetComponent<PlatformPlayer>().dead) {
            transition.GetComponent<TransitionManager>().OnDeath();
            player.GetComponent<PlatformPlayer>().hp = 3;
            player.GetComponent<PlatformPlayer>().dead = false;
            player.transform.position = startPos.position;
        }

        stoppingTime = player.GetComponent<PlatformPlayer>().stoppingTime;
        hp = player.GetComponent<PlatformPlayer>().hp;
        timeStopBar = player.GetComponent<PlatformPlayer>().timeStopBar;
        comboMeter = player.GetComponent<PlatformPlayer>().comboMeter;
    }

    public void Pause(InputAction.CallbackContext context) {
        if(context.started) {
            gamePaused = !gamePaused;
            if(!gamePaused) {
                gamePaused = true;
                StartCoroutine(PauseAnimExit());
            }
        }
    }

    public void Continue() {
        gamePaused = true;
        StartCoroutine(PauseAnimExit());
    }

    IEnumerator PauseAnimExit() {
        pauseAnimator.SetTrigger("closePause");

        yield return new WaitUntil(() => pauseAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        yield return new WaitWhile(() => pauseAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }
}
