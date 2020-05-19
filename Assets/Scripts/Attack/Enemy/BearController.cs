using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : EnemyController
{
    public bool IsActiwateSprint;
    public float Speed;

    private float _facktSpeed;
    private float _nextAttackTime;
    private bool _canAttack = true;
    //Destoyed in HitArea class

    void Start()
    {
        OnBattleEnd = new UnityEngine.Events.UnityEvent();
        _targetCharacter = GameController.Instance.PlayerCharacter;
        _controlledEnemy = gameObject.GetComponent<Enemy>();
        _controlledEnemy.OnDie.AddListener(ProcessEnemyDeath);
       if(IsActiwateSprint)
        StartCoroutine("Sprint");
       else
            _facktSpeed = Speed;
    }

    IEnumerator Sprint() //появление волков
    {
        _facktSpeed = 16;
        yield return new WaitForSeconds(1f);
        _facktSpeed = Speed;
    }

    void FixedUpdate()
    {
        if (!_controlledEnemy.GetDead() && !_isInAnimation)
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x + _facktSpeed * Time.deltaTime, gameObject.transform.localPosition.y);

            if (!_targetCharacter.GetDead() && _canAttack)
            {
            
                float sqrDstToTarget = (_targetCharacter.transform.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(_controlledEnemy.AttackDistanceThreshold, 2))
                {
                    if (Time.time > _nextAttackTime)
                    {
                        Attack();
                        _nextAttackTime = Time.time + 4;
                    }
                }
            }
        }
    }
    public void Kill()
    {
        _controlledEnemy.Kill();
    }
    public override void TakeDamage()
    {
        StartCoroutine("Slowdown");
    }

    IEnumerator Slowdown()
    {
        _facktSpeed = 9;
        _controlledEnemy.TakeDamage(1);
        yield return new WaitForSeconds(0.6f);
        _facktSpeed = Speed;
    }

    public override int GetCount()
    {
        if (_controlledEnemy.GetDead())
            return 0;
        else
            return 1;
    }

    public bool GetLifeBear()
    {
        return _controlledEnemy.GetDead();
    }

    public override void Attack()
    {
        _controlledEnemy.StartAttack(Vector3.zero);
    }

    public override bool GetActiv()
    {
        return gameObject.activeSelf;
    }

    //perform actions on player death
    public override void StartPlayerDieAnimation()
    {
        //if player has second life item 
        if (PlayerDataController.Instance.HasItem(1) != 0)
        {
            //disable attack availability 
            _canAttack = false;
            Destroy(gameObject, 3f);
        }
        else
        {
            //slow down enemy to stop at X units in fromnt of player character body

            _isInAnimation = true;
            //define positions
            _startAnimPosition = transform.position;
            _targetAnimPosition = _targetCharacter.transform.position - new Vector3(2f, 0, 0);
            _animDistance = (_targetAnimPosition - _startAnimPosition).magnitude;
            StartCoroutine(AnimatePlayerDeath());
        }
    }

    public override void SetEnemyStatic()
    {
        _controlledEnemy.SetStatic();
    }

    public override EnemyType GetEnemyType()
    {
        return _controlledEnemy.Type;
    }

    public override void ProcessEnemyDeath()
    {
        if (_controlledEnemy.GetDead())
            OnBattleEnd.Invoke();
    }
}
