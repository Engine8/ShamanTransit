using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public float Speed = 1f;

    CircleCollider2D parCollider;
    // Start is called before the first frame update
    void Start()
    {
        parCollider = transform.parent.GetComponent<CircleCollider2D>();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        bool isOk = false;
        int searchStatus = 0;
        //float wrongAngle = 0; 
        while (!isOk)
        {
            float angle = 0;
            if (searchStatus == 0)
                angle = Random.Range(0f, 360f);
            else if (searchStatus < 8)
                angle = 45f;//(wrongAngle + 180f) / 360f; 
            transform.Rotate(transform.forward, angle);
            Vector3 newPos = transform.position + transform.right * Speed * Time.fixedDeltaTime;
            if (parCollider.bounds.Contains(newPos))
            {
                isOk = true;
                transform.position = newPos;
            }
            else
            {
                //wrongAngle = angle;
                searchStatus = (searchStatus + 1) % 8;
            }
        }
    }
    */
}
