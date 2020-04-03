using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class HelpControl : MonoBehaviour
{
    public static HelpControl helpControl;
    private HelpUI _helpUI;
    public bool HelpMessageActiv { get; private set; }
    private void Awake()
    {
        if (helpControl == null)
        {
            helpControl = this;
        }
    }
    public void Help(GameObject MessageUI)
    {
        _helpUI = new HelpUI(MessageUI);
        StartCoroutine(AwakeMessage());
    }
    public void Move()
    {
        if (HelpMessageActiv && HelpUI.name == "MovingHelp")
            CloseUI();
    }
    public void CheatingAttack()
    {
        if (HelpMessageActiv && HelpUI.name == "AttackFalseHelp")
            CloseUI();
    }
    private void CloseUI()
    {
        HelpMessageActiv = false;
        Time.timeScale = 1f;
        _helpUI.setActiv(false);
    }
    IEnumerator AwakeMessage()
    {
        AppearanceFirstUI();
        Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(2f);
        HelpMessageActiv = true;
        ConcealmentFirstUI();
        _helpUI.setActiv(1, true);
        if (HelpUI.name == "AttackTrueHelp")
            Time.timeScale = 1f;
    }
    private void ConcealmentFirstUI()
    {
        StartCoroutine(AppearanceOf(_helpUI._alphaFirstUI));
    }
    private void AppearanceFirstUI()   
    {
        _helpUI.setActiv(0, true);
        StartCoroutine(AppearanceOf(_helpUI._alphaFirstUI));
    }
    IEnumerator AppearanceOf(CanvasGroup _alpha)
    {
        if (_alpha.alpha == 0)
        {
            while (_alpha.alpha < 1)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                _alpha.alpha = _alpha.alpha + 0.05f;
            }
        }
        else
        {
            while (_alpha.alpha > 0)
            {
                yield return new WaitForSecondsRealtime(0.01f);
                _alpha.alpha = _alpha.alpha - 0.05f;
            }
            _helpUI.setActiv(0, false);
        }
    }

    class HelpUI
    {
        public static string name { get; private set; }
        public CanvasGroup _alphaFirstUI;
        
        GameObject[] MessageUI =new GameObject[2];
        
        public HelpUI(GameObject helpUI)
        {
            name = helpUI.name;
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
            _alphaFirstUI = MessageUI[0].GetComponent<CanvasGroup>();
        }
        public void setActiv( bool activ)
        {
            MessageUI[0].SetActive(activ);
            MessageUI[1].SetActive(activ);
        }
        public void setActiv(int index, bool activ)
        {
            if (MessageUI[index])
                MessageUI[index].SetActive(activ);
        }
    }
}
