using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private AudioSource _musicSource;
    private List<AudioSource> _soundSources;


    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public static void Initialize()
    {
        if (_instance != null)
            return;
        
        GameObject soundManagerGameObject = new GameObject("SoundManager");
        _instance = soundManagerGameObject.AddComponent<SoundManager>();
        DontDestroyOnLoad(soundManagerGameObject);
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMusic(AudioClip music)
    {
        //stop current music 

        if (_musicSource == null)
        {
            
            _musicSource = new AudioSource();
            //setup new audio surce
        }

        _musicSource.clip = music;
        _musicSource.Play();
    }

}
