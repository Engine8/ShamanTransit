using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
{
    public Transform Arrow;
    public WolfPosition Wolfs;
    public SpriteRenderer MissAr;

    private SightScale sightScale;
   
    void Start()
    {
        sightScale = FindObjectOfType<SightScale>();
        transform.localEulerAngles = new Vector3(0, 0, 68);
    }

    void Update()
    {
          if (Input.touchCount > 0 || Input.GetMouseButtonDown(0))
          {
                if (Arrow.localEulerAngles.z >= transform.localEulerAngles.z && Arrow.localEulerAngles.z <= transform.localEulerAngles.z + 18)
                {
                    transform.localEulerAngles = new Vector3(0, 0, Random.Range(0f, 68f));
                    if (Wolfs.GetCountWolfs() >= 0)
                    {
                        Wolfs.DieWolf();
                    }
                    if (Wolfs.GetCountWolfs() == 0)
                    {
                        sightScale.Stop();
                    }
                    sightScale.BafSpeed();
                }
                else
                {
                     StartCoroutine("Miss");
                     Wolfs.Attack();
                }
          }
    }
    IEnumerator Miss()
    {
        MissAr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        MissAr.color = Color.white;
    }
}
