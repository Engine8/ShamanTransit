using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soul : MonoBehaviour
{
    public float Speed = 1f;
    public float FlyAwaySpeed = 5f;

    public enum SoulStatus
    {
        Free = 0,
        Net = 1,
        Animation = 2,
    }

    public SoulStatus Status;

    [SerializeField]
    private Vector3 _flyAwayDir;
    private bool _needToFlyAway;
    public SpriteRenderer spriteRenderer;
    CircleCollider2D parCollider;
    private float _distance;
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
        while (!isOk && !_needToFlyAway && Status == SoulStatus.Net)
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

        if (_needToFlyAway && Status == SoulStatus.Net)
        {
            Vector3 newPos;
            //main direction movement
            float mainOffset = FlyAwaySpeed * Time.fixedDeltaTime;
            newPos = transform.position + _flyAwayDir * mainOffset;
            _distance += mainOffset;

            //add random movement
            float angle = Random.Range(-30f, 210f);
            float x = Mathf.Cos(angle * Mathf.PI / 180);
            float y = Mathf.Sin(angle * Mathf.PI / 180);
            Vector3 dir = new Vector3(x, y, 0).normalized;
            newPos = newPos + dir * Speed * Time.fixedDeltaTime;
            transform.position = newPos;

            if (_distance > 15f)
                Destroy(gameObject);
        }

    }
    
    public void FlyAway(Vector3 dir)
    {
        _needToFlyAway = true;
        _flyAwayDir = dir;

    }
}
