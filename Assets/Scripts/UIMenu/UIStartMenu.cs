using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIStartMenu : MonoBehaviour
{
    public GameObject ShopMenu;
    public GameObject StartMenu;
    public void StartGame()
    {
        Debug.Log("LoadScene('play')");
       // SceneManager.LoadScene("play");
    }

    public void OpenSettings()
    {
        Debug.Log("OpenSettings");
    }
    public void OpenShop()
    {
        StartMenu.SetActive(false);
        ShopMenu.SetActive(true);
    }
    public void ClouseShop()
    {
        StartMenu.SetActive(true);
        ShopMenu.SetActive(false);
    }
}
