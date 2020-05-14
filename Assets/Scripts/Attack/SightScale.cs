using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SightScale : MonoBehaviour
{
    public Transform Arrow;
    public Transform HitArea;

    private bool _Victory;
    public float SpeedRotate;
    private float _startAngle;

    private void Start()
    {
        _startAngle = Arrow.localEulerAngles.z;

    }
    public void CalculateSpeed()
    {
        int res = Random.Range(0, 100);
        if (res < 5)
            SpeedRotate = 1 * Mathf.Sign(SpeedRotate);
        else if (res < 35)
            SpeedRotate = 2 * Mathf.Sign(SpeedRotate);
        else
            SpeedRotate = 3 * Mathf.Sign(SpeedRotate);
    }
   
    public void Stop()
    {
        _Victory = true;
        StartCoroutine("Weate");
    }

    IEnumerator Weate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (Mathf.Abs(SpeedRotate) <= 0)
                break;
            SpeedRotate = (Mathf.Abs(SpeedRotate) - 0.25f) * SpeedRotate / Mathf.Abs(SpeedRotate);
        }
        _Victory = false;
        gameObject.transform.parent.gameObject.SetActive(false);
        CalculateSpeed();
    }

    void FixedUpdate()
    {
        if (Arrow.localEulerAngles.z < _startAngle)
        {            
            SpeedRotate *= -1;
        }
        else if (Arrow.localEulerAngles.z >= (_startAngle + 88f)) 
        {
            SpeedRotate *= -1;
        }
        float rotationZ = Arrow.localEulerAngles.z + SpeedRotate;
        Arrow.localEulerAngles = new Vector3(0, 0, rotationZ);
    }
    public bool GetVictory()
    {
        return _Victory;
    }
}
