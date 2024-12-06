using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [SerializeField] Animator transition;

    public void StartGame() {
        StartCoroutine(TransitionTimer("Level1"));
    }

    public void RestartLevel() {
        StartCoroutine(TransitionTimer(SceneManager.GetActiveScene().name));
    }

    public void FromLevelSelect(string level) {
        StartCoroutine(TransitionTimer(level));
    }

    public void MainMenu() {
        StartCoroutine(TransitionTimer("StartMenu"));
    }

    public void QuitGame() {
        StartCoroutine(TransitionTimer("Quit"));
    }

    public void NextLevel(string level) {
        StartCoroutine(TransitionTimer(level));
    }

    public void OnDeath(GameObject player, Transform startPos) {
        transition.ResetTrigger("TransitionEnd");
        StartCoroutine(DeathTransition(player, startPos));
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

    IEnumerator DeathTransition(GameObject player, Transform startPos) {
        transition.SetTrigger("TransitionStart");

        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        yield return new WaitWhile(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        var platplayer = player.GetComponent<PlatformPlayer>();
        platplayer.hp = 3;
        platplayer.Dead = false;
        platplayer.animHandler.hasDied = false;
        platplayer.animHandler.animator.SetBool("dead2", false);
        player.transform.position = startPos.position;

        transition.SetTrigger("TransitionEnd");

        yield return new WaitUntil(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);

        

       yield return new WaitWhile(() => transition.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
    }
}
