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
    private float _pauseAttacksMiss = 0;
    private float _pauseAttacksHit = 0;

    void Start()
    {
        sightScale = transform.parent.GetComponent<SightScale>();
        MissAr = transform.parent.gameObject.GetComponent<Image>();
        transform.localEulerAngles = new Vector3(0, 0, 60);
    }

    public void SetEnnemy(EnemyController value)
    {
        _enumy = value;
    }

    public void Tach()
    {
        if (!sightScale.GetVictory())
        {
            GameController.Instance.SetAnimAttack();
            if (Arrow.localEulerAngles.z <= (transform.localEulerAngles.z + 3) && Arrow.localEulerAngles.z >= (sightScale.SpeedRotate > 0 ? transform.localEulerAngles.z - 16 : transform.localEulerAngles.z - 16))
            {
                if (Time.time > _pauseAttacksHit)
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
                        }
                    }
                    //sightScale.BafSpeed();
                    _pauseAttacksHit = Time.time + 0.5f;
                }
            }
            else
            {
                StartCoroutine("Miss");
                if (Time.time > _pauseAttacksMiss)
                {
                    if (_enumy.GetActiv())
                        _enumy.Attack();
                    _pauseAttacksMiss = Time.time + 0.5f;
                }
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