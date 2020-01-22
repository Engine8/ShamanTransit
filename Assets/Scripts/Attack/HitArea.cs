using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    public Transform Arrow;
    public Transform[] Enumy;

    private int countEnumy;
    private SightScale sightScale;
    void Start()
    {
        sightScale = FindObjectOfType<SightScale>();
        transform.localEulerAngles = new Vector3(0, 0, 68);
        countEnumy = Enumy.Length-1;
    }

    void Update()
    {
        if (Input.touchCount>0 || Input.GetMouseButtonDown(0))
        {
            if (Arrow.localEulerAngles.z >= transform.localEulerAngles.z && Arrow.localEulerAngles.z <= transform.localEulerAngles.z+18)
            {
                transform.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 68f));
                if (countEnumy>=0) 
                { 
                    Enumy[countEnumy].gameObject.SetActive(false);
                    --countEnumy; 
                }
                sightScale.BafSpeed();
                Debug.Log("True");
            }
        }
    }
}
