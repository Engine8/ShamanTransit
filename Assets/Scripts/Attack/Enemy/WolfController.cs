using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WolfController :  EnemyController
{
    public List<Enemy> Wolf;
    public float TimeBetweenAttacks;//скорость атаки
    private float _speedBuf;

    private Vector3[][] _positionWolf = new Vector3[4][];
    private float _nextAttackTime;
    private int _countWolf = 4;

    private bool _isAttack = false;
    private bool _CR_running = false;
    //Destroyed in HitArea class
    private void OnDestroy()
    {
        //Destroy all wolfes released from controller's transform
        foreach (var wolf in Wolf)
            Destroy(wolf.gameObject);
    }

    private void Awake()
    {
        OnBattleEnd = new UnityEvent();
    }

    void Start()
    {
        _targetCharacter = GameController.Instance.PlayerCharacter;
        _nextAttackTime = Time.time + TimeBetweenAttacks;
        for (int i = 0; i < 4; ++i)
        {
            Wolf[i].OnDie.AddListener(ProcessEnemyDeath);
            _positionWolf[i] = new Vector3[4 - i];
            for (int j = 0; j < 4 - i; ++j)
            {
                if (i != 2)
                {
                    _positionWolf[i][j] = new Vector3(Wolf[j].gameObject.transform.localPosition.x, Wolf[j].gameObject.transform.localPosition.y, 0);
                }
                else
                {
                    _positionWolf[i][0] = new Vector3(Wolf[1].gameObject.transform.localPosition.x, Wolf[1].gameObject.transform.localPosition.y, 0);
                    _positionWolf[i][1] = new Vector3(Wolf[2].gameObject.transform.localPosition.x, Wolf[2].gameObject.transform.localPosition.y, 0);
                }
            }
        }
        StartCoroutine("Sprint");
    }

    IEnumerator Sprint() //появление волков
    {
        _speedBuf = _targetCharacter.Speed * 0.6f;
        yield return new WaitForSeconds(1.6f);
        _speedBuf = 0;
    }

    public void ChendePosition()
    {
        --_countWolf;
        int indexPosition = 0;
        if (!_CR_running)
            StopCoroutine("ChangePosition");
        for (int i = 0; i < 4; ++i)
        {
            if (!Wolf[i].GetDead())
            {
                if ((4 - _countWolf) == 2)
                    StartCoroutine(ChangePosition(Wolf[i].gameObject.transform, new Vector3(_positionWolf[4 - _countWolf][indexPosition].x + 3, _positionWolf[4 - _countWolf][indexPosition].y, 0)));
                else
                    StartCoroutine(ChangePosition(Wolf[i].gameObject.transform, _positionWolf[4 - _countWolf][indexPosition]));
                ++indexPosition;
            }
        }
    }

    IEnumerator ChangePosition(Transform wolfA, Vector3 wolfB)
    {
        _CR_running = true;

        Vector3 newPositionA = new Vector3((float)System.Math.Round((double)wolfB.x, 1), (float)System.Math.Round((double)wolfB.y, 1), wolfB.z);

        while (wolfA.localPosition != newPositionA)
        {
            if (wolfA.localPosition.x != newPositionA.x)
                wolfA.localPosition += new Vector3(wolfA.localPosition.x > newPositionA.x ? -0.1f : 0.1f, 0, 0);
            if (wolfA.localPosition.y != newPositionA.y)
                wolfA.localPosition += new Vector3(0, wolfA.localPosition.y > newPositionA.y ? -0.1f : 0.1f, 0);

            wolfA.localPosition = new Vector3((float)System.Math.Round((double)wolfA.localPosition.x, 1), (float)System.Math.Round((double)wolfA.localPosition.y, 1), wolfA.localPosition.z);

            yield return new WaitForSeconds(0.01f);
        }
        _CR_running = false;
    }

    void FixedUpdate()
    {
        if (_countWolf > 0 && !_isInAnimation)
        {
            //gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x + _facktSpeed * Time.deltaTime, gameObject.transform.localPosition.y);
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x +(_targetCharacter.Speed + _speedBuf) * Time.deltaTime, gameObject.transform.localPosition.y);

            if (!_targetCharacter.GetDead())
            {
                if (!_isAttack)
                {
                    if (Time.time > _nextAttackTime)
                        Attack();

                    if ((_countWolf > 0) && ((4 - _countWolf) != 2))
                        Wolf[4 - _countWolf].transform.localPosition += new Vector3(0.2f * Time.deltaTime, 0f, 0f);
                }
            }
        }
    }
 
    public override void Attack()
    {
        StartCoroutine("PauseForAttack");
        if (_countWolf > 0)
        {
            if ((4 - _countWolf) == 2)
            {
                int rand = Random.Range(2, 4);
                Wolf[rand].StartAttack(Wolf[rand].transform.localPosition);
            }
            else
                Wolf[4 - _countWolf].StartAttack(_positionWolf[4 - _countWolf][0]);
            _nextAttackTime = Time.time + TimeBetweenAttacks;
        }
    }

    public override void AttackOnMiss()
    {
        Attack();
    }

    IEnumerator PauseForAttack()
    {
        _isAttack = true;
        yield return new WaitForSeconds(3f);
        _isAttack = false;
    }

    public override void TakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine("PauseForAttack");
        _nextAttackTime = Time.time + TimeBetweenAttacks;
        Wolf[(4 - _countWolf)].TakeDamage(1);
        
    }

    public override  int GetCount()
    {
        return _countWolf;
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
            Destroy(gameObject, 3f);
        }
        else
        {
            //slow down enemy to stop at X units in front of player character body
            _isInAnimation = true;
            //define positions
            _startAnimPosition = transform.position;
            _targetAnimPosition = _targetCharacter.transform.position - new Vector3(1f, 0, 0);
            _animDistance = (_targetAnimPosition - _startAnimPosition).magnitude;
            StartCoroutine(AnimatePlayerDeath());
        }
    }

    public override void SetEnemyStatic()
    {
        bool isFirstNotDead = true;
        for (int i = 0; i < Wolf.Count; ++i)
        {
            if (!Wolf[i].GetDead())
            {
                if (isFirstNotDead)
                {
                    Wolf[i].SetAnimationBool("IsHowl", true);
                    SoundManager.Instance.PlaySoundClip(EnterSound, true);
                    isFirstNotDead = false;
                }
                else
                    Wolf[i].SetStatic();
            }
        }
    }

    public override EnemyType GetEnemyType()
    {
        return EnemyType.Wolf;
    }

    public override void ProcessEnemyDeath()
    {
        ChendePosition();
        if (_countWolf == 0)
            OnBattleEnd.Invoke();
    }
}
