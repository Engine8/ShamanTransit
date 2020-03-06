using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //private SoundSettings _settings;

    private AudioSource _musicSource;
    public int MaxSoundsCount = 8;
    private List<AudioSource> _soundSources;

    private List<AudioClip> _loadedClips;
    private bool _loadingInProgress = false;
    
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

        foreach (AudioClip clip in GameData.Instance.GameSoundSettings.PreloadedClips)
        {
            _loadedClips.Add(clip);
        }

        _soundSources = new List<AudioSource>();
        _loadedClips = new List<AudioClip>();

        ApplySoundVolume();
        ApplyMusicVolume();

        ApplySoundMuted();
        ApplyMusicMuted();
    }

    void Update()
    {
        // Destory only one sound per frame
        AudioSource soundSource = null;

        foreach (AudioSource sound in _soundSources)
        {
            if (IsSoundFinished(sound))
            {
                soundSource = sound;
                break;
            }
        }

        if (soundSource != null)
        {
            _soundSources.Remove(soundSource);
            Destroy(soundSource);
        }
    }

    public void PlayMusicClip(AudioClip musicClip)
    {
        if (_musicSource == null)
        {    
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.priority = 0;
            _musicSource.playOnAwake = false;
            _musicSource.mute = GameData.Instance.GameSoundSettings.GetMusicMuted();
            _musicSource.ignoreListenerPause = true;
        }

        //dont't restart curent playing music
        if (_musicSource.clip != null && musicClip.name == _musicSource.clip.name)
            return;

        _musicSource.Stop();

        _musicSource.clip = musicClip;
        _musicSource.Play();
    }

    public void PlaySoundClip(AudioClip soundClip)
    {
        if (_soundSources.Count > MaxSoundsCount)
            return;

        AudioSource soundSource = gameObject.AddComponent<AudioSource>();

        soundSource.clip = soundClip;
        soundSource.priority = 0;
        soundSource.playOnAwake = false;
        soundSource.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        soundSource.volume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        soundSource.ignoreListenerPause = false;
        soundSource.Play();

        _soundSources.Add(soundSource);
    }

    public void PlaySound(string soundName)
    {
        if (_soundSources.Count > MaxSoundsCount)
        {
            return;
        }

        AudioSource soundSource = gameObject.AddComponent<AudioSource>();

        soundSource.priority = 0;
        soundSource.playOnAwake = false;
        soundSource.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        soundSource.volume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        soundSource.ignoreListenerPause = false;

        _soundSources.Add(soundSource);

        AudioClip loadedClip = TryFindSoundClipPreloaded(soundName);

        if (loadedClip != null)
        {
            soundSource.clip = loadedClip;
            soundSource.Play();
        }
        else
        {
            StartCoroutine(PlaySoundAfterLoad(soundSource, soundName));
        }
    }

    IEnumerator PlaySoundAfterLoad(AudioSource soundSource, string soundName)
    {
        while (_loadingInProgress)
        {
            yield return null;
        }
        _loadingInProgress = true;

        AudioClip soundClip = null;
        ResourceRequest request = LoadClipAsync(soundName);
        while (!request.isDone)
            yield return null;
        soundClip = (AudioClip)request.asset;

        _loadingInProgress = false;

        soundSource.clip = soundClip;
        soundSource.Play();
    }

    public void PausePausibleSounds(bool isPaused = true)
    {
        AudioListener.pause = isPaused;
    }

    private void LoadSound(string soundName)
    {
        AudioClip clip = TryFindSoundClipPreloaded(soundName);
        if (clip != null)
        {
            return;
        }
        clip = LoadClip(soundName);
        if (clip != null)
        {
            if (!clip.preloadAudioData)
                clip.LoadAudioData();
            _loadedClips.Add(clip);
        }
    }

    private void UnloadSound(string soundName)
    {
        AudioClip clipToUnload = TryFindSoundClipPreloaded(soundName);
        if (clipToUnload != null)
        {
            _loadedClips.Remove(clipToUnload);
            if (clipToUnload.preloadAudioData)
                clipToUnload.UnloadAudioData();
        }
    }

    AudioClip LoadClip(string name)
    {
        string path = "Sounds/" + name;
        AudioClip clip = Resources.Load<AudioClip>(path);
        return clip;
    }

    ResourceRequest LoadClipAsync(string name)
    {
        string path = "Sounds/" + name;
        return Resources.LoadAsync<AudioClip>(path);
    }

    AudioClip TryFindSoundClipPreloaded(string soundName)
    {
        AudioClip loadedClip = null;
        foreach (AudioSource source in _soundSources)
        {
            if (source.clip.name == soundName)
            {
                loadedClip = source.clip;
                return loadedClip;
            }
        }
        return null;
    }

    bool IsSoundFinished(AudioSource soundSource)
    {
        if (soundSource.isPlaying)
            return false;

        if (soundSource.clip.loadState == AudioDataLoadState.Loading)
            return false;

        if (!soundSource.ignoreListenerPause && AudioListener.pause)
            return false;

        return true;
    }

    public void SetMusicVolume(float volume)
    {
        GameData.Instance.GameSoundSettings.SetMusicVolume(volume);
        ApplyMusicVolume();
    }

    public float GetMusicVolume()
    {
        return GameData.Instance.GameSoundSettings.GetMusicVolume();
    }

    public void SetSoundVolume(float volume)
    {
        GameData.Instance.GameSoundSettings.SetSoundVolume(volume);
        ApplySoundVolume();
    }

    public float GetSoundVolume()
    {
        return GameData.Instance.GameSoundSettings.GetSoundVolume();
    }

    public void SetMusicMuted(bool mute)
    {
        GameData.Instance.GameSoundSettings.SetMusicMuted(mute);
        ApplyMusicMuted();
    }

    public bool GetMusicMuted()
    {
        return GameData.Instance.GameSoundSettings.GetMusicMuted();
    }

    public void SetSoundMuted(bool mute)
    {
        GameData.Instance.GameSoundSettings.SetSoundMuted(mute);
        ApplySoundMuted();
    }

    public bool GetSoundMuted()
    {
        return GameData.Instance.GameSoundSettings.GetSoundMuted();
    }

    void ApplySoundVolume()
    {
        foreach (AudioSource soundSource in _soundSources)
        {
            soundSource.volume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        }
    }

    void ApplyMusicVolume()
    {
        if (_musicSource != null)
        {
            _musicSource.volume = GameData.Instance.GameSoundSettings.GetMusicVolume();
        }
    }

    void ApplySoundMuted()
    {
        foreach (AudioSource soundSource in _soundSources)
        {
            soundSource.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        }
    }

    void ApplyMusicMuted()
    {
        if (_musicSource != null)
        {
            _musicSource.mute = GameData.Instance.GameSoundSettings.GetMusicMuted();
        }
    }
}
