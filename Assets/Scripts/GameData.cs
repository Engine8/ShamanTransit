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

    public static void Initialize()
    {
        if (_instance == null)
            _instance = new GameData();
    }

    public void SetCurrentLevel(int stage, int level)
    {
        CurrentStage = stage;
        CurrentLevel = level;
        CurrentLevelIndex = stage * 5 + level;
    }



}
