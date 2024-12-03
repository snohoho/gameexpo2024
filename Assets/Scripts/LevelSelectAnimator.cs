using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectAnimator : MonoBehaviour
{
    [SerializeField] Animator level1;
    [SerializeField] Animator level2;
    [SerializeField] Animator level3;
    [SerializeField] GameObject panel;
    [SerializeField] GameObject creditsPanel;

    private bool fading;
    private bool sliding;

    void Awake() {
        fading = true;
        sliding = false;

        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        panel.GetComponent<CanvasGroup>().alpha = 0;

        creditsPanel.GetComponent<CanvasGroup>().interactable = false;
        creditsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        creditsPanel.GetComponent<CanvasGroup>().alpha = 0;
    }

    void Update() {
        if(!fading && !sliding) {
            StartCoroutine(StartSlideAnimation(level1));
            StartCoroutine(StartSlideAnimation(level2));
            StartCoroutine(StartSlideAnimation(level3));
        }
    }

    public void TransitionToLvlSelect() {
        creditsPanel.SetActive(false);
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        StartCoroutine(FadeInLevelSelect());
    }

    public void TransitionFromLvlSelect() {
        creditsPanel.SetActive(true);
        panel.GetComponent<CanvasGroup>().interactable = false;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        StartCoroutine(FadeOutLevelSelect());
    }

    public void TransitionToCredits() {
        panel.SetActive(false);
        creditsPanel.GetComponent<CanvasGroup>().interactable = true;
        creditsPanel.GetComponent<CanvasGroup>().blocksRaycasts = true;
        StartCoroutine(FadeInCredits());
    }

    public void TransitionFromCredits() {
        panel.SetActive(true);
        creditsPanel.GetComponent<CanvasGroup>().interactable = false;
        creditsPanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        StartCoroutine(FadeOutCredits());
    }

    IEnumerator FadeInLevelSelect() {
        fading = true;
        while(panel.GetComponent<CanvasGroup>().alpha < 1f) {
            panel.GetComponent<CanvasGroup>().alpha += 0.01f;
            yield return new WaitForEndOfFrame();
        }
        fading = false;
    }

    IEnumerator FadeOutLevelSelect() {
        while(panel.GetComponent<CanvasGroup>().alpha > 0f) {
            panel.GetComponent<CanvasGroup>().alpha -= 0.01f;
            yield return new WaitForEndOfFrame();
        }

        sliding = false;
        level1.ResetTrigger("start");
        level2.ResetTrigger("start");
        level3.ResetTrigger("start");
    }

    IEnumerator FadeInCredits() {
        while(creditsPanel.GetComponent<CanvasGroup>().alpha < 1f) {
            creditsPanel.GetComponent<CanvasGroup>().alpha += 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeOutCredits() {
        while(creditsPanel.GetComponent<CanvasGroup>().alpha > 0f) {
            creditsPanel.GetComponent<CanvasGroup>().alpha -= 0.01f;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator StartSlideAnimation(Animator animator) {
        sliding = true;
        animator.SetTrigger("start");

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
    }
}
