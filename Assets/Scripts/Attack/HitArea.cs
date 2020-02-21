using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitArea : MonoBehaviour 
{
    public Transform Arrow;

    private BearControler Enemy;
    private Image MissAr;
    private SightScale sightScale;
    void Start()
    {
        MissAr = transform.parent.gameObject.GetComponent<Image>();
        sightScale = FindObjectOfType<SightScale>();
        transform.localEulerAngles = new Vector3(0, 0, 60);
    }
    public void Tach()
    {
        if (Arrow.localEulerAngles.z <= transform.localEulerAngles.z && Arrow.localEulerAngles.z >= transform.localEulerAngles.z - 18)
        {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(20f, 90f));
            if (Enemy.gameObject.activeSelf)
            {
                if (Enemy.GetCount() >= 0)
                {
                    Enemy.TakeDamage();
                }
                if (Enemy.GetCount() == 0)
                {
                    sightScale.Stop();
                }
            }
            sightScale.BafSpeed();
        }
        else
        {
            StartCoroutine("Miss");
            if (Enemy.gameObject.activeSelf)
                Enemy.Attack();
        }
    }
    IEnumerator Miss()
    {
        MissAr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        MissAr.color = Color.white;
    }
}
