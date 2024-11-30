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

    IEnumerator TransitionTimer(string sceneToLoad) {
        transition.SetTrigger("TransitionStart");

        yield return new WaitForSeconds(2f);
        
        SceneManager.LoadScene(sceneToLoad);
    }
}
