using System.Collections;
using System.Collections.Generic;
using Unity.UIWidgets.ui;
using UnityEngine;

public class Boss : EnemyController
{  
    enum LineAttack {Line1= -127,Line2=-206,Line3=-299 };

    public int helf;

    private float _speedBuf;

    private PlayerController Player;
    private float nextAttackTime;
    private bool _canAttack = true;
    private bool Dead = false;
    //Destoyed in HitArea class

    void Start()
    {
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
        }
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
