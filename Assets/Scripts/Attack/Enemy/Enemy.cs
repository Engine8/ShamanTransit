using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EnemyType
{
    Wolf = 0,
    Bear = 1,
    Boss = 2,
}


public class Enemy : MonoBehaviour
{
    public EnemyType Type;
    public float AttackDistanceThreshold;//дистанция атаки
    public int Damage;
    public int Health;
    public Sprite SpecialSprite;

    public UnityEvent OnDie = new UnityEvent();

    private bool _dead = false;
    private PlayerController _targetCharacter;
    private Animator _animator;
    private Vector3 _oldPosition;
    
    void Start()
    {
        _targetCharacter = GameController.Instance.PlayerCharacter;
        _animator = GetComponent<Animator>();

        if (Type == EnemyType.Bear && PlayerDataController.Instance.HasItem(3) != 0)
        {
            _animator.SetBool("IsSpecial", true);
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            renderer.sprite = SpecialSprite;
        }
    }

    public bool GetDead()
    {
        return _dead;
    }

    public void StartAttack(Vector3 oldPosition)
    {
        _oldPosition = oldPosition;
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        Vector3 originalPosition = _oldPosition!=Vector3.zero? _oldPosition : transform.localPosition;
        Vector3 dirToTarget = (new Vector3(_targetCharacter.transform.position.x-0.5f, _targetCharacter.transform.position.y+0.5f, _targetCharacter.transform.position.z) - transform.position).normalized;
        Vector3 attackPosition = transform.localPosition + dirToTarget * 3;

        float attackSpeed = 3;
        float percent = 0;

        bool hasAppLiedDamage = false;
        float JumpDistance = _targetCharacter.transform.position.x - transform.position.x;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppLiedDamage)
            {
                hasAppLiedDamage = true;

                _targetCharacter.TakeDamage(Damage);
            }
            percent += Time.deltaTime * attackSpeed;
          
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * Mathf.Abs(JumpDistance);
            transform.localPosition = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0 && !_dead)
        {
            Die();
        }
    }

    void Die()
    {
        _dead = true;
        if (Type != EnemyType.Bear || PlayerDataController.Instance.HasItem(3) == 0)
            _animator.SetBool("IsDead", true);
        if (Type == EnemyType.Wolf)
        {
            /*release wolf from controller gameobject
             * WolfController has reference to this object in Wolf array and will delete it in OnDestoy
             */
            transform.SetParent(null, true);
        }
        OnDie.Invoke();
    }

    public void OnDeadAnimationEnd()
    {
        gameObject.SetActive(false);
    }

    public void SetStatic()
    {
        _animator.SetBool("IsStatic", true);
    }

    public void SetAnimationBool(string boolName, bool value)
    {
        _animator.SetBool(boolName, value);
    }
}
