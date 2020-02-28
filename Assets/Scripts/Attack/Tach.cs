using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tach : MonoBehaviour, IPointerDownHandler
{
    public HitArea hit;
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if(FindObjectOfType<HitArea>())
             hit.Tach();
    }

}
