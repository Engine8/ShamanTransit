using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class MapScript : MonoBehaviour
{
    private int _currentStage;
    private int _currentLevel;

    public Color ActiveLevelColor;
    public Color InactiveLevelColor;
    public GameObject LevelHandler;

    public LoadingComponent loadingComponent;  
    // Start is called before the first frame update
    void Awake()
    {

        _currentStage = PlayerDataController.Instance.Data.CurrentStage;
        _currentLevel = PlayerDataController.Instance.Data.CurrentLevel;

        //initialize previous levels buttons
        for (int i = 0; i < LevelHandler.transform.childCount; ++i)
        {
            //get level button prefab
            Transform levelButtonPrefabTransform = LevelHandler.transform.GetChild(i);

            bool isGlowActive;
            bool isActiveSelf;
            Color targetColor;
            if (i < _currentLevel)
            {
                isGlowActive = false;
                isActiveSelf = true;
                targetColor = ActiveLevelColor;
            }
            else if (i == _currentLevel)
            {
                isGlowActive = true;
                isActiveSelf = true;
                targetColor = ActiveLevelColor;
            }
            else
            {
                isGlowActive = false;
                isActiveSelf = false;
                targetColor = InactiveLevelColor;
            }

            //off the glow effect
            levelButtonPrefabTransform.GetChild(0).gameObject.SetActive(isGlowActive);
            //set button state
            levelButtonPrefabTransform.GetChild(1).GetComponent<Button>().interactable = isActiveSelf;
            //set color of image on the button
            levelButtonPrefabTransform.GetChild(1).GetComponent<Image>().color = targetColor;
        }
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
