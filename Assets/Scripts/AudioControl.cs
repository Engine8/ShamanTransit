using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    public static AudioControl audioControl;
    AudioSource audioSource;
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        if(PlayerPrefs.GetInt("Audio") == 1)
        {
            Debug.Log("true");
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
    public void AudioOnOff()
    {
        if(PlayerPrefs.GetInt("Audio") == 1)
        {
            Debug.Log("true");
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

}
