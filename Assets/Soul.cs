using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public float Speed = 1f;

    public SpriteRenderer spriteRenderer;
    CircleCollider2D parCollider;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        parCollider = transform.parent.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        bool isOk = false;
        int searchStatus = 0;
        //float wrongAngle = 0; 
        while (!isOk)
        {
            float angle;
            Vector3 newPos = new Vector3(); ;
            if (searchStatus == 0)
            {
                angle = Random.Range(0f, 360f);
                transform.Rotate(transform.forward, angle);
                newPos = transform.position + transform.right * Speed * Time.fixedDeltaTime;
            }
            else
            {
                Vector3 dirToCenter = (parCollider.bounds.center - transform.position).normalized;
                newPos = transform.position + dirToCenter * Speed * searchStatus * Time.fixedDeltaTime;
            }
            if (parCollider.bounds.Contains(newPos))
            {
                isOk = true;
                transform.position = newPos;
                searchStatus = 0;
            }
            else
            {
                searchStatus += 1;
            }
        }
    }
    
}
