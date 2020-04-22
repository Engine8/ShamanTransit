using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Wolf = 0,
    Bear = 1,
}


public class Enemy : MonoBehaviour
{
    public EnemyType Type;
    public int startingHealth;
    public float attackDistanceThreshold;//дистанция атаки
    public int Damage; //урон

    public Sprite SpecialSprite;

    private Transform targetPlayer;
    private int Health;
    private bool dead=false;
    private PlayerController HealsPlayer;
    private Animator _animator;

    float timeBetweenAttacks = 1; //скорость атаки
    float nextAttackTime;
    Vector3 _oldPosition;


    void Start()
    {
        targetPlayer = FindObjectOfType<PlayerController>().transform;
        HealsPlayer = targetPlayer.GetComponent<PlayerController>();
        _animator = GetComponent<Animator>();
        Health = startingHealth;

        if (Type == EnemyType.Bear && PlayerDataController.Instance.HasItem("Something strange") != 0)
        {
            _animator.SetBool("IsSpecial", true);
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = SpecialSprite;
        }
    }

    public bool GetDead()
    {
        return dead;
    }
    public void StartAttack(Vector3 oldPosition)
    {
        _oldPosition = oldPosition;
        StartCoroutine(Attack());
    }
    IEnumerator Attack() //интерфейс перебора колекций
    {
        Vector3 originalPosition = _oldPosition!=Vector3.zero? _oldPosition : transform.localPosition;
        Vector3 dirToTarget = (new Vector3(targetPlayer.position.x-0.5f, targetPlayer.position.y+0.5f, targetPlayer.position.z) - transform.position).normalized;
        Vector3 attackPosition = transform.localPosition + dirToTarget * 3;

        float attackSpeed = 3;
        float percent = 0;

        bool hasAppLiedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppLiedDamage)
            {
                hasAppLiedDamage = true;

                HealsPlayer.TakeDamage(Damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 3.5f;
            transform.localPosition = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0 && !dead)
        {
            Die();
        }
    }
    void Die()
    {
        dead = true;
        if (Type != EnemyType.Bear || PlayerDataController.Instance.HasItem("Something strange") == 0)
            _animator.SetBool("IsDead", true);
        if (FindObjectOfType<WolfController>())
        {
            /*release wolf from controller gameobject
             * WolfController has reference to this object in Wolf array and will delete it in OnDestoy
             */
            transform.SetParent(null, true);
            //gameObject.SetActive(false);
           // FindObjectOfType<WolfController>().ChendePosition();
        }
        // GameObject.Destroy(gameObject, 5f);
    }

    public void OnDeadAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
