using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] Animator transition;

    public void StartGame() {
        StartCoroutine(TransitionTimer("TestScene"));
    }

    public void LevelSelect() {
        //StartCoroutine(TransitionTimer("TestScene"));
    }

    public void MainMenu() {
        StartCoroutine(TransitionTimer("StartMenu"));
    }

    public void QuitGame() {
        StartCoroutine(TransitionTimer("Quit"));
    }

    IEnumerator TransitionTimer(string sceneToLoad) {
        transition.SetTrigger("TransitionStart");

        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        yield return new WaitWhile(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        
        if(sceneToLoad == "Quit") {
            Application.Quit();
        }

        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneToLoad);
    }
}
