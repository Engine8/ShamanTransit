using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private static GameData _instance;
    public static GameData Instance
    {
        get
        {
            return _instance;
        }
    }

    public int CurrentStage;
    public int CurrentLevel;
    public int CurrentLevelIndex;

    private bool _vibrationStatus = true;
    public bool VibrationStatus
    {
        get
        {
            return _vibrationStatus;
        }
    }

    private SoundSettings _soundSettings;
    public SoundSettings GameSoundSettings
    {
        get
        {
            return _soundSettings;
        }
    }

    public static void Initialize()
    {
        if (_instance == null)
            _instance = new GameData();

        _instance._soundSettings = Resources.Load<SoundSettings>("SoundSettings");
        _instance._soundSettings.Load();

        _instance._vibrationStatus = PlayerPrefs.GetInt("VibrationStatus", 1) == 1;
    }

    public void SetVibtationStatus(bool status)
    {
        _vibrationStatus = status;
        PlayerPrefs.SetInt("VibrationStatus", _vibrationStatus ? 1 : 0);
    }

    public void SetCurrentLevel(int stage, int level)
    {
        CurrentStage = stage;
        CurrentLevel = level;
        CurrentLevelIndex = stage * 5 + level;
    }



}
