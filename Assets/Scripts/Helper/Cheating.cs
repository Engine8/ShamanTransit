using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cheating : MonoBehaviour, IPointerDownHandler
{
    private HitArea hit;
    private void Start()
    {
        hit = FindObjectOfType<HitArea>();
    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (FindObjectOfType<HitArea>())
        {
            HelpControl.helpControl.CheatingAttack();
            hit.Tach();
        }
    }
}
