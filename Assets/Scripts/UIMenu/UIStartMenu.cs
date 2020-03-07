using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartMenu : MonoBehaviour
{
    public GameObject ShopMenu;
    public GameObject StartMenu;
    public GameObject InfoMenu;
    public GameObject MusicOff;
    public GameObject SoundOff;
    public GameObject VibrationOff;
    public LoadingComponent loadingComponent;

    public AudioClip Music;

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

        SoundManager.Instance.PlayMusicClip(Music);
    }

    public void StartGame()
    {
        Debug.Log("LoadScene('LevelScene')");
        SceneManager.LoadScene("LevelScene");
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
