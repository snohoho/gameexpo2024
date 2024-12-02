using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject player; 
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Animator pauseAnimator;
    public bool stoppingTime;
    public int hp;
    public float timeStopBar;
    public int comboMeter;
    public bool gamePaused;

    void Awake() {
        hp = player.GetComponent<PlatformPlayer>().hp;
        timeStopBar = player.GetComponent<PlatformPlayer>().timeStopBar;
        comboMeter = player.GetComponent<PlatformPlayer>().comboMeter;

        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    void Start() {
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if(gamePaused) {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;

            pauseAnimator.SetTrigger("openPause");
            Cursor.lockState = CursorLockMode.None;
            return;
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
