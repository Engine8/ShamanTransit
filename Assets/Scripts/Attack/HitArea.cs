using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitArea : MonoBehaviour 
{
    public Transform Arrow;

    private EnemyController _enumy;
    private Image MissAr;
    private SightScale sightScale;
    private CinemachineVirtualCamera vcam;
    void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();
        vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 1f;
        MissAr = transform.parent.gameObject.GetComponent<Image>();
        sightScale = FindObjectOfType<SightScale>();
        transform.localEulerAngles = new Vector3(0, 0, 60);
    }
    public void SetEnnemy(EnemyController vavue)
    {
        _enumy = vavue;
    }
    public void Tach()
    {
        if (Arrow.localEulerAngles.z <= transform.localEulerAngles.z && Arrow.localEulerAngles.z >= transform.localEulerAngles.z - 18)
        {
            transform.localEulerAngles = new Vector3(0, 0, Random.Range(20f, 90f));
            if (_enumy.GetActiv())
            {
                if (_enumy.GetCount() >= 0)
                {
                    _enumy.TakeDamage();
                }
                if (_enumy.GetCount() == 0)
                {
                    sightScale.Stop();
                    vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.6f;
                }
            }
            sightScale.BafSpeed();
        }
        else
        {
            StartCoroutine("Miss");
            if (_enumy.GetActiv())
                _enumy.Attack();
        }
    }
    IEnumerator Miss()
    {
        MissAr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        MissAr.color = Color.white;
    }
}
