using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cheating : MonoBehaviour, IPointerDownHandler
{
    public HitArea hit;
    private void Start()
    {
        hit = FindObjectOfType<HitArea>();
    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (hit)
        {
            HelpControl.helpControl.CheatingAttack();
            hit.Tach();
        }
    }
}
