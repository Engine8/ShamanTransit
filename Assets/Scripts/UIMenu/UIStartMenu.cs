using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartMenu : MonoBehaviour
{
    public GameObject ShopMenu;
    public GameObject StartMenu;
    public GameObject InfoMenu;
    public GameObject AudioOff;
    public LoadingComponent loadingComponent;
    private void Awake()
    {
        //load playerData or set default values
        PlayerDataController.Initialize();
        PlayerDataController.Instance.LoadData();
    }

    private void Start()
    {
        if (PlayerPrefs.GetInt("Audio") == 1)
        {
            AudioOff.SetActive(false);
        }
        else
        {
            AudioOff.SetActive(true);
        }
    }
    public void StartGame()
    {
        Debug.Log("LoadScene('play')");
        SceneManager.LoadScene("play");
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
    public void AudioOffON()
    {
        if (!AudioOff.active)
        {
            AudioOff.SetActive(true);
            PlayerPrefs.SetInt("Audio", 0);
        }
        else 
        { 
            AudioOff.SetActive(false);
            PlayerPrefs.SetInt("Audio", 1);
        }
    }
}
