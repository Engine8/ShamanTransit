//Based on Gasparfx SoundManager project: https://github.com/Gasparfx/SoundManager

//Licence
/*
The MIT License(MIT)

Copyright(c) 2016 Gasparfx

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //variable for main music
    private LongSound _currentMusic;

    //variables for short, discrete sounds (buttons, hits and so on)
    public int MaxSoundsCount = 8;
    private List<Sound> _sounds;

    //variable for long, continious sounds (ambient, wind blows and so on) 
    private Dictionary<string, LongSound> _longSounds;

    private List<AudioClip> _loadedClips;
    private bool _loadingInProgress = false;

    List<LongSoundFadingOut> _longSoundFadingsOut = new List<LongSoundFadingOut>();

    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public class LongSound
    {
        public string Name;
        public AudioSource Source;

        public float Timer;
        public float FadingTime;
        public float TargetVolume;
        public bool FadingIn;
    }

    public class LongSoundFadingOut
    {
        private string _name;
        public AudioSource Source;

        public float Timer;
        public float FadingTime;
        public float StartVolume;
    }

    public class Sound
    {
        public string Name;
        public AudioSource Source;
        public bool IsLoading;
        public IEnumerator LoadingCoroutine;
        public bool IsPossessedLoading;
        public float SelfVolume;
        public bool IsAttachedToTransform;
        public Transform Attach;
        public bool IsValid;
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

    private void Start()
    {
        foreach (AudioClip clip in GameData.Instance.GameSoundSettings.PreloadedClips)
        {
            _loadedClips.Add(clip);
        }

        _sounds = new List<Sound>();
        _longSounds = new Dictionary<string, LongSound>();
        _loadedClips = new List<AudioClip>();

        ApplySoundVolume();
        ApplyMusicVolume();

        ApplySoundMuted();
        ApplyMusicMuted();
    }



    void Update()
    {
        // Destory only one sound per frame
        Sound soundToDelete = null;

        foreach (Sound sound in _sounds)
        {
            if (IsSoundFinished(sound))
            {
                soundToDelete = sound;
                break;
            }
        }

        if (soundToDelete != null)
        {
            _sounds.Remove(soundToDelete);
            Destroy(soundToDelete.Source);
        }

        if (GameData.Instance.GameSoundSettings.AutoPause)
        {
            bool curPause = Time.timeScale < 0.1f;
            if (curPause != AudioListener.pause)
            {
                AudioListener.pause = curPause;
            }
        }

        for (int i = 0; i < _longSoundFadingsOut.Count; i++)
        {
            LongSoundFadingOut longSound = _longSoundFadingsOut[i];
            if (longSound.Source == null)
            {
                _longSoundFadingsOut.RemoveAt(i);
                i--;
            }
            else
            {
                longSound.Timer += Time.unscaledDeltaTime;
                _longSoundFadingsOut[i] = longSound;
                if (longSound.Timer >= longSound.FadingTime)
                {
                    Destroy(longSound.Source);
                    _longSoundFadingsOut.RemoveAt(i);
                    i--;
                }
                else
                {
                    float k = Mathf.Clamp01(longSound.Timer / longSound.FadingTime);
                    longSound.Source.volume = Mathf.Lerp(longSound.StartVolume, 0, k);
                }
            }
        }

        if (_currentMusic != null && _currentMusic.FadingIn)
        {
            _currentMusic.Timer += Time.unscaledDeltaTime;
            if (_currentMusic.Timer >= _currentMusic.FadingTime)
            {
                _currentMusic.Source.volume = _currentMusic.TargetVolume;
                _currentMusic.FadingIn = false;
            }
            else
            {
                float k = Mathf.Clamp01(_currentMusic.Timer / _currentMusic.FadingTime);
                _currentMusic.Source.volume = Mathf.Lerp(0, _currentMusic.TargetVolume, k);
            }
        }

        foreach (var longSound in _longSounds)
        { 
            if (longSound.Value.FadingIn)
            {
                longSound.Value.Timer += Time.deltaTime;
                if (longSound.Value.Timer >= longSound.Value.FadingTime)
                {
                    longSound.Value.Source.volume = longSound.Value.TargetVolume;
                    longSound.Value.FadingIn = false;
                }
                else
                {
                    float k = Mathf.Clamp01(longSound.Value.Timer / longSound.Value.FadingTime);
                    longSound.Value.Source.volume = Mathf.Lerp(0, longSound.Value.TargetVolume, k);
                }
            }
        }
    }

    public static void Pause()
    {
        if (GameData.Instance.GameSoundSettings.AutoPause)
            return;

        AudioListener.pause = true;
    }

    public static void UnPause()
    {
        if (GameData.Instance.GameSoundSettings.AutoPause)
            return;

        AudioListener.pause = false;
    }



    public void PlayMusicClip(AudioClip musicClip)
    {
        if (_currentMusic != null && _currentMusic.Name == musicClip.name)
        {
            return;
        }

        StopMusic();

        AudioSource musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.priority = 0;
        musicSource.playOnAwake = false;
        musicSource.mute = GameData.Instance.GameSoundSettings.GetMusicMuted();
        musicSource.ignoreListenerPause = true;
        musicSource.clip = musicClip;
        musicSource.Play();

        musicSource.volume = 0;

        _currentMusic = new LongSound();
        _currentMusic.Source = musicSource;
        _currentMusic.FadingIn = true;
        _currentMusic.TargetVolume = GameData.Instance.GameSoundSettings.GetMusicVolume();
        _currentMusic.Timer = 0;
        _currentMusic.FadingTime = GameData.Instance.GameSoundSettings.MusicFadeTime;
    }

    void StopMusic()
    {
        if (_currentMusic != null)
        {
            StartFadingOutMusic();
            _currentMusic = null;
        }
    }

    void StartFadingOutMusic()
    {
        if (_currentMusic != null)
        {
            LongSoundFadingOut fader = new LongSoundFadingOut();
            fader.Source = _currentMusic.Source;
            fader.FadingTime = GameData.Instance.GameSoundSettings.MusicFadeTime;
            fader.Timer = 0;
            fader.StartVolume = _currentMusic.Source.volume;
            _longSoundFadingsOut.Add(fader);
        }
    }

    public void PlayLongSoundClip(string name, AudioClip clip, bool pausable, float fadeTime)
    {
        if (_longSounds.ContainsKey(name))
        {
            StopLongSound(name);
        }

        AudioSource longSoundSource = gameObject.AddComponent<AudioSource>();
        longSoundSource.loop = true;
        longSoundSource.priority = 0;
        longSoundSource.playOnAwake = false;
        longSoundSource.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        longSoundSource.ignoreListenerPause = !pausable;
        longSoundSource.clip = clip;
        longSoundSource.Play();

        longSoundSource.volume = 0;

        LongSound longSound = new LongSound();
        longSound.Source = longSoundSource;
        longSound.FadingIn = true;
        longSound.TargetVolume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        longSound.Timer = 0;
        longSound.FadingTime = fadeTime;

        _longSounds.Add(name, longSound);
    }

    public void StopLongSound(string name)
    {
        if (_longSounds.ContainsKey(name))
        {
            StartFadingOutLongSound(name);
            _longSounds.Remove(name);
        }
    }

    void StartFadingOutLongSound(string name)
    {
        LongSoundFadingOut fader = new LongSoundFadingOut();
        LongSound sound = _longSounds[name];
        fader.Source = sound.Source;
        fader.FadingTime = sound.FadingTime;
        fader.Timer = 0;
        fader.StartVolume = sound.Source.volume;
        _longSoundFadingsOut.Add(fader);
    }

    public void PlaySoundClip(AudioClip soundClip, bool pausable)
    {
        if (_sounds.Count > MaxSoundsCount)
            return;

        Sound sound = new Sound();
        sound.Name = soundClip.name;
        sound.SelfVolume = 1;

        AudioSource soundSource = gameObject.AddComponent<AudioSource>();

        sound.Source = soundSource;

        soundSource.clip = soundClip;
        soundSource.priority = 0;
        soundSource.playOnAwake = false;
        soundSource.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        soundSource.volume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        soundSource.ignoreListenerPause = !pausable;
        soundSource.Play();

        _sounds.Add(sound);
    }

    public void PlaySound(string soundName, bool pausable)
    {
        if (_sounds.Count > MaxSoundsCount)
        {
            return;
        }

        Sound sound = new Sound();
        sound.Name = soundName;
        sound.SelfVolume = 1f;

        AudioSource soundSource = gameObject.AddComponent<AudioSource>();

        soundSource.priority = 0;
        soundSource.playOnAwake = false;
        soundSource.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        soundSource.volume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        soundSource.ignoreListenerPause = !pausable;

        sound.Source = soundSource;

        _sounds.Add(sound);

        AudioClip loadedClip = TryFindSoundClipPreloaded(soundName);

        if (loadedClip != null)
        {
            soundSource.clip = loadedClip;
            soundSource.Play();
        }
        else
        {
            sound.LoadingCoroutine = PlaySoundAfterLoad(sound);
            StartCoroutine(sound.LoadingCoroutine);
        }
    }

    IEnumerator PlaySoundAfterLoad(Sound sound)
    {
        sound.IsLoading = true;

        while (_loadingInProgress)
        {
            yield return null;
        }
        _loadingInProgress = true;

        sound.IsPossessedLoading = true;
        AudioClip soundClip = null;
        ResourceRequest request = LoadClipAsync(sound.Name);
        while (!request.isDone)
            yield return null;
        soundClip = (AudioClip)request.asset;

        sound.IsPossessedLoading = false;
        _loadingInProgress = false;

        sound.IsLoading = false;
        sound.Source.clip = soundClip;
        sound.Source.Play();
    }

    void StopSound(Sound sound)
    {
        if (sound.IsLoading)
        {
            StopCoroutine(sound.LoadingCoroutine);
            if (sound.IsPossessedLoading)
                _loadingInProgress = false;

            sound.IsLoading = false;
        }
        else
            sound.Source.Stop();
    }

    public void StopAllSounds()
    {
        foreach (Sound sound in _sounds)
        {
            StopSound(sound);
        }
        foreach(string sound in _longSounds.Keys)
        {
            StopLongSound(sound);
        }
    }

    void StopAllPausableSounds()
    {
        foreach (Sound sound in _sounds)
        {
            if (!sound.Source.ignoreListenerPause)
            {
                StopSound(sound);
            }
        }
    }

    bool IsSoundFinished(Sound sound)
    {
        if (sound.IsLoading)
            return false;

        if (sound.Source.isPlaying)
            return false;

        if (sound.Source.clip.loadState == AudioDataLoadState.Loading)
            return false;

        if (!sound.Source.ignoreListenerPause && AudioListener.pause)
            return false;

        return true;
    }


    private void LoadSoundClip(string soundName)
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

    private void UnloadSoundClip(string soundName)
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
        foreach (AudioClip loadedClip  in _loadedClips)
        {
            if (loadedClip.name == soundName)
            {
                return loadedClip;
            }
        }
        return null;
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
        foreach (Sound sound in _sounds)
        {
            sound.Source.volume = GameData.Instance.GameSoundSettings.GetSoundVolume();
        }
    }

    void ApplyMusicVolume()
    {
        if (_currentMusic != null)
        {
            _currentMusic.Source.volume = GameData.Instance.GameSoundSettings.GetMusicVolume();
        }
    }

    void ApplySoundMuted()
    {
        foreach (Sound sound in _sounds)
        {
            sound.Source.mute = GameData.Instance.GameSoundSettings.GetSoundMuted();
        }
    }

    void ApplyMusicMuted()
    {
        if (_currentMusic != null)
        {
            _currentMusic.Source.mute = GameData.Instance.GameSoundSettings.GetMusicMuted();
        }
    }
}
  



