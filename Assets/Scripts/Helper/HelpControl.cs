using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HelpControl : MonoBehaviour
{
    public static HelpControl helpControl;
    private HelpUI _helpUI;
    private void Awake()
    {
        if (helpControl == null)
        {
            helpControl = this;
        }
    }
    public void Help(GameObject MessageUI)
    {
        Debug.Log("Help " + MessageUI.name);
        _helpUI = new HelpUI(MessageUI);
        StartCoroutine(AwakeMessage());
    }
    public void Move()
    {
        Time.timeScale = 1f;
        _helpUI.setActiv(false);
    }
    IEnumerator AwakeMessage()
    {
        _helpUI.setActiv(0, true);
        Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(3f);
        _helpUI.setActiv(0, false);
        _helpUI.setActiv(1, true);
    }
    class HelpUI
    {
        GameObject[] MessageUI =new GameObject[2];
        public HelpUI(GameObject helpUI)
        {
            if (helpUI.transform.childCount == 2)
            {
                MessageUI[0] = helpUI.transform.GetChild(0).gameObject;
                MessageUI[1] = helpUI.transform.GetChild(1).gameObject;
            }
            else
            {
                MessageUI[0] = helpUI.transform.GetChild(0).gameObject;
                MessageUI[1] = null;
            }
        }
        public void setActiv( bool activ)
        {
            MessageUI[0].SetActive(activ);
            MessageUI[1].SetActive(activ);
        }
        public void setActiv(int index, bool activ)
        {
            if(MessageUI[index])
                MessageUI[index].SetActive(activ);
        }
    }
}
