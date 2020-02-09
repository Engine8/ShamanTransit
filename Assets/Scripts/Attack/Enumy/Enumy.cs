using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enumy : MonoBehaviour
{
    public float startingHealth;
    public float attackDistanceThreshold;//дистанция атаки
    public float damage; //урон
    public Transform targetPlayer;


    private float health;
    private bool dead;
    private PlayerControl HealsPlayer;

    void Start()
    {
        HealsPlayer = targetPlayer.GetComponent<PlayerControl>();
        health = startingHealth;
    }

    void Update()
    {
      
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
        Vector3 attackPosition = transform.localPosition + dirToTarget*4;


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
    public void Die()
    {
        
        dead = true;

        if (FindObjectOfType<WolfPosition>())
        {
            gameObject.SetActive(false);
            FindObjectOfType<WolfPosition>().ChendePosition();
        }
        // GameObject.Destroy(gameObject, 5f);
    }
}
