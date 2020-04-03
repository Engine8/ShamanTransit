using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class Tach : MonoBehaviour, IPointerDownHandler
{
    public HitArea hit;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            hit.Tach();
    }
    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (FindObjectOfType<HitArea>()) 
             hit.Tach();
    }

}
