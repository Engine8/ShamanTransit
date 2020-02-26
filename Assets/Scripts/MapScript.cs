using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MapScript : MonoBehaviour
{
    private int _currentStage;
    private int _currentLevel;

    public Color CurrentLevelColor;
    public LoadingComponent loadingComponent;  
    // Start is called before the first frame update
    void Awake()
    {

        //_currentStage = PlayerDataController.Instance.Data.CurrentStage;
        //_currentLevel = PlayerDataController.Instance.Data.CurrentLevel;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevel(string levelName)
    {
        string[] data = levelName.Split('-');
        GameData.Instance.SetCurrentLevel(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
        loadingComponent.StartLoadLevel("LevelScene");
    }
    public void StartMenu()
    {
        loadingComponent.StartLoadLevel("Menu");
    }
}
