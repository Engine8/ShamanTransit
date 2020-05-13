using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.ui;
using UnityEngine;

public class Boss : EnemyController
{
    public List<float> LineAttack = new List<float>(new float[3]{-1.62f, -2.52f, -3.62f});
    public float[,]  LineSize = { {4f, 1.5f }, {4.5f,2f},{ 5f,2.2f} };
    private string[] _layerName = { "Line1", "Line2", "Line3" };
    private GameObject[] _attackWarning;
    public GameObject[] AttackPrefab;

    public int Health;

    private float _speedBuf;
    public float TimeBetweenAttacks;//скорость атаки

    private PlayerController _player;
    private float _nextAttackTime=10;
    private bool _canAttack = true;
    private bool _dead = false;
    //Destoyed in HitArea class

    void Start()
    {
        _attackWarning = new GameObject[transform.childCount];
        for (int i=0;i< transform.childCount; i++)
            _attackWarning[i] = transform.GetChild(i).gameObject;

        _player = GameController.Instance.PlayerCharacter;
        transform.position = new Vector2(_player.transform.position.x - 13, 0);

        StartCoroutine("Sprint");
 
    }

    IEnumerator Sprint() //появление волков
    {
        _speedBuf = 6;
        yield return new WaitForSeconds(1f);
        _speedBuf = 0;
    }
    void FixedUpdate()
    {
        if (!_dead && !_isInAnimation&& !_player.GetDead())
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x +( _player.Speed+_speedBuf) * Time.deltaTime, gameObject.transform.localPosition.y);

            //run phase
            if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.BossRun && !_player.GetDead())
            {
                if (Time.time > _nextAttackTime)
                {
                    StartCoroutine("Warning");
                    _nextAttackTime = Time.time + TimeBetweenAttacks;
                }
            }
            //attack phase
            else if (GameController.Instance.CurrentGameStatus == GameController.GameStatus.Attack && !_player.GetDead())
            {

            }
        }
    }

    IEnumerator Warning() //появление волков
    {
        int value = Random.Range(0, 2);
        int position = Random.Range(0, 3);
        _attackWarning[value].SetActive(true);
        _attackWarning[value].transform.position = new Vector2(_attackWarning[value].transform.position.x, LineAttack[position]);
        _attackWarning[value].transform.localScale = new Vector2(_attackWarning[value].transform.localScale.x , LineSize[position, value]);
        _attackWarning[value].GetComponent<SpriteRenderer>().sortingLayerName = _layerName[position];
        yield return new WaitForSeconds(2f);
        GameObject atack =  Instantiate(AttackPrefab[value]);
        atack.transform.position = new Vector3(_attackWarning[value].transform.position.x, _attackWarning[value].transform.position.y,0f);
        atack.GetComponent<SpriteRenderer>().sortingLayerName = _layerName[position];
        Destroy(atack,3f);
        _attackWarning[value].SetActive(false);
    }

    public override void TakeDamage()
    {
        if (Health <= 0)
            _dead = true;
        else
            Health--;
    }

    public override int GetCount()
    {
        if (_dead)
            return 0;
        else
            return 1;
    }

    public override void Attack() { }

    public override bool GetActiv()
    {
        return gameObject.activeSelf;
    }


}
