using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State { Idle, Chasing, Attacking }; //перечесление событие атаки героя 
    State currentState;

    public Transform targetPlayer;

    public float attackDistanceThreshold;//дистанция атаки
    public float timeBetweenAttacks;//скорость атаки
    public float damage = 1; //урон

    public event System.Action<bool> OnStop;


   

    float nextAttackTime;
    float myCollisionRadius;
    PlayerControl HealsPlayer;
    EnumyControl enumyControl;

    void Start()
    {
        enumyControl = transform.GetComponent<EnumyControl>();
        HealsPlayer = targetPlayer.GetComponent<PlayerControl>();
        currentState = State.Chasing;
    }

    void Update()
    {
        if (!HealsPlayer.GetDead())
        {
            if (Time.time > nextAttackTime)
            {

                float sqrDstToTarget = (targetPlayer.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshold, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
        else
            enumyControl.OnStop(HealsPlayer.GetDead());
    }
    IEnumerator Attack() //интерфейс перебора колекций
    {
        currentState = State.Attacking;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (targetPlayer.position - transform.position).normalized;
        Vector3 attackPosition = targetPlayer.position - dirToTarget * (myCollisionRadius);


        float attackSpeed = 3;
        float percent = 0;

      
        bool hasAppLiedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppLiedDamage)
            {
                hasAppLiedDamage = true;

                HealsPlayer.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
           
            yield return null;
        }

        currentState = State.Chasing;

    }
}
