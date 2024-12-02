using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectAnimator : MonoBehaviour
{
    [SerializeField] Animator level1;
    [SerializeField] Animator level2;
    [SerializeField] Animator level3;
    [SerializeField] GameObject panel;

    private bool fading;
    private bool sliding;

    void Awake() {
        fading = true;
        sliding = false;
        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        panel.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update() {
        if(!fading && !sliding) {
            StartCoroutine(StartSlideAnimation(level1));
            StartCoroutine(StartSlideAnimation(level2));
            StartCoroutine(StartSlideAnimation(level3));
        }
    }

    public void TransitionToLvlSelect() {
        Debug.Log("test1");
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        StartCoroutine(FadeInLevelSelect());
    }

    IEnumerator FadeInLevelSelect() {
        fading = true;
        while(panel.GetComponent<CanvasGroup>().alpha < 1f) {
            panel.GetComponent<CanvasGroup>().alpha += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        fading = false;
        sliding = false;
    }

    IEnumerator StartSlideAnimation(Animator animator) {
        sliding = true;
        animator.SetTrigger("start");

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
    }
}
