using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource source;
    
    public void PlayClip(AudioClip clip) {
        AudioSource audio = Instantiate(source);

        audio.clip = clip;
        audio.Play();
        float length = audio.clip.length;

        Destroy(audio.gameObject, length);
    }
}
