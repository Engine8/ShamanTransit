using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.ui;
using UnityEngine;

public class Boss : EnemyController
{
    float[]  LineAttack = {-1.8f, -2.52f, -3.62f };
    float[,]  LineSize = { {4f, 1.5f }, {4.5f,2f},{ 5f,2.2f} };
    string[] LayerName = { "Line1", "Line2", "Line3" };
    GameObject[] AttackWarning;
    public int helf;

    private float _speedBuf;
    public float timeBetweenAttacks;//скорость атаки

    private PlayerController Player;
    private float nextAttackTime=10;
    private bool _canAttack = true;
    private bool Dead = false;
    //Destoyed in HitArea class

    void Start()
    {
        AttackWarning = new GameObject[transform.childCount];
        for (int i=0;i< transform.childCount; i++)
            AttackWarning[i] = transform.GetChild(i).gameObject;

        Player = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        transform.position = new Vector2(Player.transform.position.x - 13, 0);

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
        if (!Dead && !_isInAnimation)
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x +( Player.Speed+_speedBuf) * Time.deltaTime, gameObject.transform.localPosition.y);
            if (!Player.GetDead())
            {
                if (Time.time > nextAttackTime) 
                {
                    StartCoroutine("Warning");
                    nextAttackTime = Time.time + timeBetweenAttacks;
                }
            }
        }
    }
    IEnumerator Warning() //появление волков
    {
        int value = Random.Range(0, 2);
        int position = Random.Range(0, 3);
        AttackWarning[value].SetActive(true);
        AttackWarning[value].transform.position = new Vector2(AttackWarning[value].transform.position.x, LineAttack[position]);
        AttackWarning[value].transform.localScale = new Vector2(AttackWarning[value].transform.localScale.x , LineSize[position, value]);
        AttackWarning[value].GetComponent<SpriteRenderer>().sortingLayerName = LayerName[position];
        yield return new WaitForSeconds(2f);  
        AttackWarning[value].SetActive(false);
    }
    public override void TakeDamage()
    {
        if (helf <= 0)
            Dead = true;
        else
            helf--;
    }


    public override int GetCount()
    {
        if (Dead)
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
