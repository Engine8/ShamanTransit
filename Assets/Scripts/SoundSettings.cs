using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundSettings", menuName = "SoundSettings", order = 51)]
public class SoundSettings : ScriptableObject
{
    public List<AudioClip> PreloadedClips;

    private float _musicVolume;
    private float _soundVolume;

    private bool _isMusicMuted;
    private bool _isSoundMuted;

    public void Save()
    {
        PlayerPrefs.SetFloat("MusicVolume", _musicVolume);
        PlayerPrefs.SetFloat("SoundVolume", _soundVolume);

        PlayerPrefs.SetInt("IsMusicMuted", _isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt("IsSoundMuted", _isSoundMuted ? 1 : 0);
    }

    public void Load()
    {
        _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        _soundVolume = PlayerPrefs.GetFloat("SoundVolume", 1);

        _isMusicMuted = PlayerPrefs.GetInt("IsMusicMuted", 0) == 1;
        _isSoundMuted = PlayerPrefs.GetInt("IsSoundMuted", 0) == 1;
    }


    //setters and getters
    public void SetMusicVolume(float volume)
    {
        _musicVolume = volume;
        Save();
    }

    public float GetMusicVolume()
    {
        return _musicVolume;
    }

    public void SetSoundVolume(float volume)
    {
        _soundVolume = volume;
        Save();
    }

    public float GetSoundVolume()
    {
        return _soundVolume;
    }

    public void SetMusicMuted(bool mute)
    {
        _isMusicMuted = mute;
        Save();
    }

    public bool GetMusicMuted()
    {
        return _isMusicMuted;
    }

    public void SetSoundMuted(bool mute)
    {
        _isSoundMuted = mute;
        Save();
    }

    public bool GetSoundMuted()
    {
        return _isSoundMuted;
    }
}
