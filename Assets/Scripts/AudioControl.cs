using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
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

}
