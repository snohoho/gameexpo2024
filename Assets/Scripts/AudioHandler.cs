using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource continuousSource;
    [SerializeField] private PlatformPlayer player;
    private bool continuousSound;
    
    public void PlayClip(AudioClip clip) {
        AudioSource audio = Instantiate(source);

        audio.clip = clip;
        audio.Play();
        float length = audio.clip.length;

        Destroy(audio.gameObject, length);
    }

    public void PlayClipContinuous(AudioClip clip, bool controlBool) {
        SetContinuousSound(controlBool);
        AudioSource audio = Instantiate(continuousSource);

        audio.clip = clip;
        audio.Play();

        StartCoroutine(DestroyContinuousClip(audio));
    }

    public void StopClipContinuous(bool controlBool) {
        SetContinuousSound(controlBool);
    }

    public void SetContinuousSound(bool sound) {
        continuousSound = sound;
    }

    IEnumerator DestroyContinuousClip(AudioSource audio) {   
        yield return new WaitWhile(() => continuousSound);

        Destroy(audio.gameObject);
    }
}
