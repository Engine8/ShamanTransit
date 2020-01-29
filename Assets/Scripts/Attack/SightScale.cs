using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightScale : MonoBehaviour
{
    public Transform Arrow;
    public Transform HitArea;
    public int SpeedRotate;
    void Start()
    {
        
    }
    public void BafSpeed()
    {
        SpeedRotate = (int)Random.Range(1f, 4f) * (SpeedRotate / SpeedRotate);
    }
    
    void Update()
    {
        if (Arrow.localEulerAngles.z >= 90f && SpeedRotate > 0)
        {
            SpeedRotate *= -1;
        }
        else if (Arrow.localEulerAngles.z <= 1f || Arrow.localEulerAngles.z >= 350f && SpeedRotate < 0)
        {
            SpeedRotate *= -1;
        }
        float rotationZ = Arrow.localEulerAngles.z + SpeedRotate;
        Arrow.localEulerAngles = new Vector3(0, 0, rotationZ);
    }
}
