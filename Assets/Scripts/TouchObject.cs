using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TouchObject : MonoBehaviour
{
    public UnityEvent OnClick;
    public bool IsActive;

    public void OnMouseDown()
    {
        if (IsActive && OnClick != null)
        {
            Debug.Log("Click mouse!");
            OnClick.Invoke();
        }
    }
}
