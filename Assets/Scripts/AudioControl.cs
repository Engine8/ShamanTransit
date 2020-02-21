using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioControl : MonoBehaviour
{
    AudioSource audio;
    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
        if(PlayerPrefs.GetInt("Audio") == 1)
        {
            Debug.Log("true");
            audio.Play();
        }
        else
        {
            audio.Stop();
        }
    }

}
