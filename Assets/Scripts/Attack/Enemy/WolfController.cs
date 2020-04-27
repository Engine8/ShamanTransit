using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class WolfController :  EnemyController
{
    public Enemy[] Wolf;
    public float timeBetweenAttacks;//скорость атаки
    public float speed;
    private float _facktSpeed;
    private Vector3[][] _positionWolf = new Vector3[4][];
    private float nextAttackTime;
    private int countWolf = 4;
    private PlayerController HealsPlayer;

    bool isAttack = false;
    bool CR_running = false;
    //Destroyed in HitArea class
    private void OnDestroy()
    {
        //Destroy all wolfes released from controller's transform
        foreach (var wolf in Wolf)
            Destroy(wolf.gameObject);
    }

    void Start()
    {
        _facktSpeed = speed;
        HealsPlayer = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        nextAttackTime = Time.time + timeBetweenAttacks;
        for (int i = 0; i < 4; ++i)
        {
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
        _facktSpeed = 16;
        yield return new WaitForSeconds(1.4f);
        _facktSpeed = speed;

    }

    public void ChendePosition()
    {
        --countWolf;
       
        int indexPosition = 0;
        if (!CR_running)
            StopCoroutine("ChangePosition");
        for (int i = 0; i < 4; ++i)
        {
            if (!Wolf[i].GetDead())
            {
                if ((4 - countWolf) == 2)
                    StartCoroutine(ChangePosition(Wolf[i].gameObject.transform, new Vector3(_positionWolf[4 - countWolf][indexPosition].x + 3, _positionWolf[4 - countWolf][indexPosition].y, 0)));
                else
                    StartCoroutine(ChangePosition(Wolf[i].gameObject.transform, _positionWolf[4 - countWolf][indexPosition]));
                ++indexPosition;
            }
        }
    }

    IEnumerator ChangePosition(Transform wolfA, Vector3 wolfB)
    {
        CR_running = true;

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
        CR_running = false;
    }

    void FixedUpdate()
    {
        if (countWolf > 0 && !_isInAnimation)
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x + _facktSpeed * Time.deltaTime, gameObject.transform.localPosition.y);

            if (!HealsPlayer.GetDead())
            {
                if (!isAttack)
                {
                    if (Time.time > nextAttackTime)
                        Attack();

                    if ((countWolf > 0) && ((4 - countWolf) != 2))
                        Wolf[4 - countWolf].transform.localPosition += new Vector3(0.2f * Time.deltaTime, 0f, 0f);
                }
            }
        }
    }
 
    public override void Attack()
    {
        StartCoroutine("PauseForAttack");
        if (countWolf > 0)
        {
            if ((4 - countWolf) == 2)
            {
                int rand = Random.Range(2, 4);
                Wolf[rand].StartAttack(Wolf[rand].transform.localPosition);
            }
            else
                Wolf[4 - countWolf].StartAttack(_positionWolf[4 - countWolf][0]);
            nextAttackTime = Time.time + timeBetweenAttacks;
        }
    }

    IEnumerator PauseForAttack()
    {
        isAttack = true;
        yield return new WaitForSeconds(3f);
        isAttack = false;
    }

    public override void TakeDamage()
    {
        StopAllCoroutines();
        StartCoroutine("PauseForAttack");
        nextAttackTime = Time.time + timeBetweenAttacks;
        Wolf[(4 - countWolf)].TakeDamage(1);
        ChendePosition();
    }

    public override  int GetCount()
    {
        return countWolf;
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
            _targetAnimPosition = HealsPlayer.transform.position - new Vector3(2f, 0, 0);
            _animDistance = (_targetAnimPosition - _startAnimPosition).magnitude;
            StartCoroutine(AnimatePlayerDeath());
        }
    }

    public override void SetEnemyStatic()
    {
        foreach(var wolf in Wolf)
        {
            if (!wolf.GetDead())
                wolf.SetStatic();
        }
    }
}
