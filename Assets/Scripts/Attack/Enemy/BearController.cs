using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : EnemyController
{
    public float speed;
    private float _facktSpeed;

    private PlayerController HealsPlayer;
    private Enemy Bear;
    private float nextAttackTime;
    private bool _canAttack = true;
    //Destoyed in HitArea class

    void Start()
    {
        HealsPlayer = FindObjectOfType<PlayerController>().GetComponent<PlayerController>();
        Debug.Log(gameObject.GetComponent<Enemy>());
        Bear = gameObject.GetComponent<Enemy>();
        StartCoroutine("Sprint");
    }

    IEnumerator Sprint() //появление волков
    {
        _facktSpeed = 16;
        yield return new WaitForSeconds(1f);
        _facktSpeed = speed;
    }

    void FixedUpdate()
    {
        if (!Bear.GetDead() && !_isInAnimation)
        {
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x + _facktSpeed * Time.deltaTime, gameObject.transform.localPosition.y);

            if (!HealsPlayer.GetDead() && _canAttack)
            {
                float sqrDstToTarget = (HealsPlayer.transform.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(Bear.attackDistanceThreshold, 2))
                {
                    if (Time.time > nextAttackTime)
                    {
                        Bear.StartAttack(Vector3.zero);
                        nextAttackTime = Time.time + 20;
                    }
                }
            }
        }
    }
    public override void TakeDamage()
    {
        StartCoroutine("Slowdown");
    }

    IEnumerator Slowdown()
    {
        Debug.Log(_facktSpeed);
        float oldSpeed = _facktSpeed;
        _facktSpeed = 9;
        Bear.TakeDamage(1);
        yield return new WaitForSeconds(0.6f);
        _facktSpeed = oldSpeed;
    }

    public override int GetCount()
    {
        if (Bear.GetDead())
            return 0;
        else
            return 1;
    }

    public bool GetLifeBear()
    {
        return Bear.GetDead();
    }

    public override void Attack() { }

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
            //destroy?
        }
        else
        {
            //slow down enemy to stop at X units in fromnt of player character body

            _isInAnimation = true;
            //define positions
            _startAnimPosition = transform.position;
            _targetAnimPosition = HealsPlayer.transform.position - new Vector3(2f, 0, 0);
            _animDistance = (_targetAnimPosition - _startAnimPosition).magnitude;
            StartCoroutine(AnimatePlayerDeath());
        }
    }

    public IEnumerator AnimatePlayerDeath()
    {
        float curTime = 0;
        bool end = false;
        while (!end)
        {
            curTime += Time.deltaTime;
            if (curTime > RunTime)
            {
                curTime = RunTime;
                end = true;
            }

            float tVal = curTime / RunTime;
            //define x position
            float x = Mathf.Lerp(_startAnimPosition.x, _targetAnimPosition.x, RunCurve.Evaluate(tVal));
            gameObject.transform.position = new Vector3(x, _startAnimPosition.y, _startAnimPosition.z);
            yield return null;
        }
        Bear.SetStatic();
    }
}
