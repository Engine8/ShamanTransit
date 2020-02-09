using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float startingHealth;
    public GameObject uiGame;
    public int speed;

   
    private float health;
    private bool dead;

    void Start()
    {
        health = startingHealth;
    }
    void FixedUpdate()
    {
        if (!dead)
        {
            this.gameObject.transform.localPosition = new Vector2(this.gameObject.transform.localPosition.x + speed * Time.deltaTime, this.gameObject.transform.localPosition.y);
        }
    }
    public bool GetDead() 
    {
        return dead;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !dead)
        {
            Die();
        }
    }
    void Die()
    {
        uiGame.SetActive(false);
        dead = true;
       // GameObject.Destroy(gameObject);
    }

}
