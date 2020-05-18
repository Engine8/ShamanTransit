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

    public List<int> Phases = new List<int>();
    public int CurrentAttackPhase = 0;

    private float _speedBuf;
    public float TimeBetweenAttacks;//скорость атаки

    private float _nextAttackTime=10;
    private bool _canAttack = true;
    private bool _dead = false;

    public UnityEvent OnAttackPhaseEnded;

    void Awake()
    {
        _controlledEnemy = gameObject.GetComponent<Enemy>();
        _attackWarning = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
            _attackWarning[i] = transform.GetChild(i).gameObject;
    }

    void Start()
    {
        //OnBattleEnd = new UnityEvent();
        _targetCharacter = GameController.Instance.PlayerCharacter;
        transform.position = new Vector2(_targetCharacter.transform.position.x - 13, 0);
        OnAttackPhaseEnded = new UnityEvent();
        _controlledEnemy.OnDie.AddListener(ProcessEnemyDeath);
        StartCoroutine(Sprint(6));
 
    }

    IEnumerator Sprint(float speed) //появление волков
    {
        _speedBuf = speed;
        yield return new WaitForSeconds(1f);
        _speedBuf = 0;
    }
    
    IEnumerator Slowdown(float speed) //появление волков
    {
        _speedBuf = -speed;
        yield return new WaitForSeconds(1f);
        _speedBuf = 0;
    }
    void FixedUpdate()
    {
        if (!_dead && !_isInAnimation && !_targetCharacter.GetDead())
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x +(_targetCharacter.Speed+_speedBuf) * Time.deltaTime, gameObject.transform.localPosition.y);

            //run phase
            if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.BossRun && !_targetCharacter.GetDead())
            {
                if (Time.time > _nextAttackTime)
                {
                    StartCoroutine("Warning");
                    _nextAttackTime = Time.time + TimeBetweenAttacks;
                }
            }
            //attack phase
            else if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.Attack && 
                     !_targetCharacter.GetDead() &&
                     _canAttack)
            {
                StartCoroutine(LineAttackAnimation());
            }
        }
    }

    IEnumerator Warning()
    {
        int value = Random.Range(0, 2);
        int position = Random.Range(0, 3);
        _attackWarning[value].SetActive(true);
        _attackWarning[value].transform.position = new Vector2(_attackWarning[value].transform.position.x, LineAttack[position]);
        _attackWarning[value].transform.localScale = new Vector2(_attackWarning[value].transform.localScale.x , LineSize[position, value]);
        _attackWarning[value].GetComponent<SpriteRenderer>().sortingLayerName = _layerName[position];
        yield return new WaitForSeconds(2f);
        GameObject attack =  Instantiate(AttackPrefab[value]);
        attack.transform.position = new Vector3(_attackWarning[value].transform.position.x, _attackWarning[value].transform.position.y,0f);
        attack.GetComponent<SpriteRenderer>().sortingLayerName = _layerName[position];
        attack.layer = GameController.Instance.DefinePhysicsLayerByString(_layerName[position]);
        Destroy(attack,2f);
        _attackWarning[value].SetActive(false);
    }

    public override void TakeDamage()
    {
        _controlledEnemy.TakeDamage(1);
        if (!_controlledEnemy.GetDead() && Phases[CurrentAttackPhase] >= _controlledEnemy.Health)
        {
            ++CurrentAttackPhase;
            OnBattleEnd.Invoke();
        }
    }

    public override int GetCount()
    {
        if (_dead)
            return 0;
        else
            return 1;
    }

    public override void Attack() { }

    public IEnumerator LineAttackAnimation()
    {
        //StartCoroutine(Slowdown(4)); 
        _canAttack = false;
        Debug.Log("LineAttack");
        _targetCharacter.TakeDamage(1);
        yield return new WaitForSeconds(10f);
        //StartCoroutine(Sprint(4));
        //OnBattleEnd.Invoke();
        _canAttack = true;
    }

    public override bool GetActiv()
    {
        return gameObject.activeSelf;
    }

    public override EnemyType GetEnemyType()
    {
        return _controlledEnemy.Type;
    }
    class Fas
    {
        public float timeFas;
        private GameObject[] _attackWarning;
        public GameObject[] AttackPrefab;
        public bool isspeeeed;
    }

    public override void ProcessEnemyDeath()
    {
        if (_controlledEnemy.GetDead())
            OnBattleEnd.Invoke();
    }
}
