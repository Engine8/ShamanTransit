using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearController : EnemyController
{
    public int speed;
   
    private PlayerControl HealsPlayer;
    private Enemy Bear;
    private float nextAttackTime;
    void Start()
    {
        HealsPlayer = FindObjectOfType<PlayerControl>().GetComponent<PlayerControl>();
        Bear = gameObject.GetComponent<Enemy>();
    }
    void FixedUpdate()
    {
        if (!HealsPlayer.GetDead())
        {
            if (!Bear.GetDead())
            {
                this.gameObject.transform.localPosition = new Vector2(this.gameObject.transform.localPosition.x + speed * Time.deltaTime, this.gameObject.transform.localPosition.y);
                float sqrDstToTarget = (HealsPlayer.transform.position - transform.position).sqrMagnitude;

                if (sqrDstToTarget < Mathf.Pow(Bear.attackDistanceThreshold, 2))
                {
                    if (Time.time > nextAttackTime)
                    {
                        Bear.StartAttack();
                        nextAttackTime = Time.time + 20;
                    }
                }
            }
            else
            {
                Bear.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
            }
        }
    }
    public override void TakeDamage()
    {
        StartCoroutine("Slowdown");
    }
    IEnumerator Slowdown()
    {
        Debug.Log(speed);
        int oldSpeed = speed;
        speed = 9;
        Bear.TakeDamage(1);
        yield return new WaitForSeconds(0.6f);
        speed = oldSpeed;
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
}
