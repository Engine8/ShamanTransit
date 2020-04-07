using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class TouchObject : MonoBehaviour, IPointerDownHandler
{
    public UnityEvent OnClick;
    public bool IsActive;

    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (IsActive && OnClick != null)
            OnClick.Invoke();
    }
}
