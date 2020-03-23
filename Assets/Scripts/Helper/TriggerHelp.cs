using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class TriggerHelp : MonoBehaviour
{
    public GameObject MessageUI;
    public delegate void MessageStartHelp(GameObject ingex);
    public event MessageStartHelp message;

    private void Start()
    {
        if(HelpControl.helpControl!=null)
            message += HelpControl.helpControl.Help;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        message.Invoke(MessageUI);
    }
}
