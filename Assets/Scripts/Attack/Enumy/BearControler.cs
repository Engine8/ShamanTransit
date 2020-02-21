using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearControler : MonoBehaviour
{
    public int speed;
    public PlayerControl HealsPlayer;
    //public float timeBetweenAttacks;//скорость атаки

    private Enumy Bear;
    private float nextAttackTime;
    void Start()
    {
        // StartCoroutine("Sprint");
        Bear = gameObject.GetComponent<Enumy>();
    }
    IEnumerator Sprint() //появление волков
    {
        speed = 16;
        yield return new WaitForSeconds(1.3f);
        speed = 10;
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
    public void TakeDamage()
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
    public int GetCount()
    {   
        if(Bear.GetDead())
            return 0;
        else
            return 1;
    }
    public bool GetLifeBear()
    {
       return Bear.GetDead();
    }
    public void Attack()
    { }
 }
