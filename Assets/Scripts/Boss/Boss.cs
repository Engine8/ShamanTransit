using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.ui;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class Boss : EnemyController
{
    public List<float> LineAttack = new List<float>(new float[3]{-1.62f, -2.52f, -3.62f});
    public float[,]  LineSize = { {4f, 1.5f }, {4.5f,2f},{ 5f,2.2f} };
    private string[] _layerName = { "Line1", "Line2", "Line3" };
    private GameObject[] _attackWarning;
    public GameObject[] AttackPrefab;
    public GameObject WolfPrefab;
    private BearController _wolf;
    public List<int> Phases = new List<int>();
    public int CurrentAttackPhase = 0;

    private float _speedBuf;
    public float TimeBetweenLineAttacksMin;//скорость атаки на линии
    public float TimeBetweenLineAttacksMax;

    public float TimeBetweenAttacksMin;//скорость атаки во время боя
    public float TimeBetweenAttacksMax;

    private float _nextAttackTime;

    private bool _canAttack = true;
    private bool _batl = false;


    public UnityEvent OnPawnSpawn;

    void Awake()
    {
        _controlledEnemy = gameObject.GetComponent<Enemy>();
        _attackWarning = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            _attackWarning[i] = transform.GetChild(i).gameObject;
        OnPawnSpawn = new UnityEvent();
        OnBattleEnd = new UnityEvent();
    }

    void Start()
    {
        _targetCharacter = GameController.Instance.PlayerCharacter;
        transform.position = new Vector2(_targetCharacter.transform.position.x - 13, 0);
        _controlledEnemy.OnDie.AddListener(ProcessEnemyDeath);
        GameController.Instance.OnGameModeChanged.AddListener(UpdateAttackTimeOnGameStatusChange);
        StartCoroutine(Sprint(6));
        UpdateAttackTimeOnGameStatusChange();
    }

    IEnumerator Sprint(float speed)
    {
        _speedBuf = speed;
        yield return new WaitForSeconds(1f);
        _speedBuf = 0;
    }
    
    IEnumerator Slowdown(float speed)
    {
        _speedBuf = -speed;
        yield return new WaitForSeconds(1f);
        _speedBuf = 0;
    }

    public void UpdateAttackTimeOnGameStatusChange()
    {
        if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.BossRun)
            _nextAttackTime = Time.time + Random.Range(TimeBetweenLineAttacksMin, TimeBetweenLineAttacksMax);
        else if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.Attack)
            _nextAttackTime = Time.time + Random.Range(TimeBetweenAttacksMin, TimeBetweenAttacksMax);
    }

    void FixedUpdate()
    {
        if (!_controlledEnemy.GetDead() && !_isInAnimation && !_targetCharacter.GetDead())
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x +(_targetCharacter.Speed+_speedBuf) * Time.deltaTime, gameObject.transform.localPosition.y);

            //run phase
            if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.BossRun && !_targetCharacter.GetDead())
            {
                if (Time.time > _nextAttackTime)
                {
                    StartCoroutine(PerformLineAttack());
                    _nextAttackTime = Time.time + Random.Range(TimeBetweenLineAttacksMin, TimeBetweenLineAttacksMax);
                }
            }
            //attack phase
            else if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.Attack &&
                     !_targetCharacter.GetDead() &&
                     _canAttack)
            {
                if(!_batl)
                    StartBattle();
                if (Time.time > _nextAttackTime)
                {
                    //StartAttackAnimation();
                    AttackAnimation();
                }
            }
        }
    }

    IEnumerator PerformLineAttack()
    {
        int value = Random.Range(0, 2);
        int position = Random.Range(0, 3);
        _attackWarning[value].SetActive(true);
        _attackWarning[value].transform.position = new Vector2(_attackWarning[value].transform.position.x, LineAttack[position]);
        _attackWarning[value].transform.localScale = new Vector2(_attackWarning[value].transform.localScale.x , LineSize[position, value]);
        _attackWarning[value].GetComponent<SpriteRenderer>().sortingLayerName = _layerName[position];
        yield return new WaitForSeconds(1f);
        GameObject attack =  Instantiate(AttackPrefab[value]);
        attack.transform.position = new Vector3(_attackWarning[value].transform.position.x, _attackWarning[value].transform.position.y,0f);
        attack.GetComponent<SpriteRenderer>().sortingLayerName = _layerName[position];
        attack.layer = GameController.Instance.DefinePhysicsLayerByString(_layerName[position]);
        Destroy(attack,0.7f);
        _attackWarning[value].SetActive(false);
    }

    public override void TakeDamage()
    {
        _controlledEnemy.TakeDamage(1);
        //if (_wolf != null)
        //    _wolf.TakeDamage();
       
        if (!_controlledEnemy.GetDead() && Phases[CurrentAttackPhase] >= _controlledEnemy.Health)
        {
            ++CurrentAttackPhase;
            EndBattle();
        }
    }

    public override int GetCount()
    {
        if (_controlledEnemy.GetDead())
            return 0;
        else
            return 1;
    }

    public override void Attack() { }

    public void StartBattle()
    {
        _batl = true;
        StartCoroutine(Slowdown(4));
        //AttackAnimation();
    }

    void AttackAnimation()
    {
        _canAttack = false;
        _wolf = Instantiate(WolfPrefab).GetComponent<BearController>();
        _wolf.OnBattleEnd.AddListener(ProcessPawnDeath);
        _wolf.transform.position = new Vector3(transform.position.x+2, transform.position.y - 2, 0f);
        OnPawnSpawn.Invoke();
    }

    public void EndBattle()
    {
        _batl = false; 
        StartCoroutine(Sprint(4));
        OnBattleEnd.Invoke();
    }

    public override bool GetActiv()
    {
        return gameObject.activeSelf;
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

    //perform actions on player death
    public override void StartPlayerDieAnimation()
    {
        //if pawn on scene then it should run away
        if (_wolf != null)
        {
            _wolf.RunAway();
            _wolf = null;
            _canAttack = true;
        }

        //stop boss
        _isInAnimation = true;
        _startAnimPosition = transform.position;
        _targetAnimPosition = _targetCharacter.transform.position - new Vector3(1f, 0, 0);
        _animDistance = (_targetAnimPosition - _startAnimPosition).magnitude;
        StartCoroutine(AnimatePlayerDeath());
    }

    public override void SetEnemyStatic()
    {

    }

    public void ContinueBattle()
    {
        UpdateAttackTimeOnGameStatusChange();
        _isInAnimation = false;
    }

    public EnemyController GetPawn()
    {
        return _wolf;
    }

    public void ProcessPawnDeath()
    {
        //StartCoroutine(Sprint(4));
        _wolf = null;
        UpdateAttackTimeOnGameStatusChange();
        _canAttack = true;
    }
}
