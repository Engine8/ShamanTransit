﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float startingHealth;
    public float attackDistanceThreshold;//дистанция атаки
    public float damage; //урон
   
    
    private Transform targetPlayer;
    private float health;
    private bool dead=false;
    private PlayerController HealsPlayer;

    void Start()
    {
        targetPlayer = FindObjectOfType<PlayerController>().transform;
        HealsPlayer = targetPlayer.GetComponent<PlayerController>();
        health = startingHealth;
    }

    public bool GetDead()
    {
        return dead;
    }
    public void StartAttack()
    {
        StartCoroutine(Attack());
    }
    IEnumerator Attack() //интерфейс перебора колекций
    {
        Vector3 originalPosition = transform.localPosition;
        Vector3 dirToTarget = (targetPlayer.position - transform.position).normalized;
        Vector3 attackPosition = transform.localPosition + dirToTarget * 4;

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
            transform.localPosition = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    void Die()
    {
        dead = true;
        if (FindObjectOfType<WolfController>())
        {
            gameObject.SetActive(false);
            FindObjectOfType<WolfController>().ChendePosition();
        }
        // GameObject.Destroy(gameObject, 5f);
    }
}
