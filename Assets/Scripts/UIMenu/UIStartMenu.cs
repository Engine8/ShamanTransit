﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartMenu : MonoBehaviour
{
    public GameObject ShopMenu;
    public GameObject SettingsMenu;
    public GameObject SettingsMenuButton;
    public GameObject StartMenu;
    public GameObject InfoMenu;
    public GameObject MusicOff;
    public GameObject SoundOff;
    public GameObject VibrationOff;
    public LoadingComponent loadingComponent;

    public GameObject PlayButton;
    public GameObject MapButton;
    public GameObject InfoButton;
    public GameObject SettingsButton;
    public GameObject ShopButton;

    public AudioClip MenuMusic;

    private void Awake()
    {

        //load playerData or set default values
        PlayerDataController.Initialize();
        PlayerDataController.Instance.LoadData();

        GameData.Initialize();
        SoundManager.Initialize();
    }

    private void Start()
    {
        MusicOff.SetActive(SoundManager.Instance.GetMusicMuted());
        SoundOff.SetActive(SoundManager.Instance.GetSoundMuted());
        VibrationOff.SetActive(!GameData.Instance.VibrationStatus);


        SoundManager.Instance.PlayMusicClip(MenuMusic);
        
        //first game entry
        if (PlayerDataController.Instance.Data.CurrentLevel == 0)
        {
            ShopButton.SetActive(false);
            MapButton.SetActive(false);
            InfoButton.SetActive(false);
        }
    }

    public void StartGame()
    {
        Debug.Log("LoadScene('LevelScene')");
        //SceneManager.LoadScene("LevelScene");

        //set current level to next available level
        GameData.Instance.SetCurrentLevel(PlayerDataController.Instance.Data.CurrentStage, PlayerDataController.Instance.Data.CurrentLevel);
        loadingComponent.StartLoadLevel("LevelScene");
    }

    public void Map()
    {
        loadingComponent.StartLoadLevel("Map");
        //SceneManager.LoadScene("Map");
    }

    public void OpenInfo()
    {
        StartMenu.SetActive(false);
        InfoMenu.SetActive(true);
    }

    public void ClouseInfo()
    {
        StartMenu.SetActive(true);
        InfoMenu.SetActive(false);
    }

    public void ClouseShop()
    {
        StartMenu.SetActive(true);
        ShopMenu.SetActive(false);
    }

    public void OpenShop()
    {
        StartMenu.SetActive(false);
        ShopMenu.SetActive(true);
    }
    public void OpenSettings()
    {
        StartMenu.SetActive(false);
        SettingsMenu.SetActive(true);
    }
    public void SettingsOff()
    {
        StartMenu.SetActive(true);
        SettingsMenu.SetActive(false);
    }
    public void MusicOffOn()
    {
        MusicOff.SetActive(!MusicOff.activeSelf);
        SoundManager.Instance.SetMusicMuted(MusicOff.activeSelf);
    }

    public void SoundOffOn()
    {
        SoundOff.SetActive(!SoundOff.activeSelf);
        SoundManager.Instance.SetSoundMuted(SoundOff.activeSelf);
    }

    public void VibrationOffOn()
    {
        VibrationOff.SetActive(!VibrationOff.activeSelf);
        GameData.Instance.SetVibtationStatus(!VibrationOff.activeSelf);
    }
}
