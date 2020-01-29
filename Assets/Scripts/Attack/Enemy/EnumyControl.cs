using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumyControl : MonoBehaviour
{
    public int speed;

    private bool playerDead;
    void Start()
    {
        speed = 15;
        StartCoroutine("Attack");
    }
    IEnumerator Attack() //интерфейс перебора колекций
    {
        yield return new WaitForSeconds(1.2f);
        speed = 11;
    }
    void Update()
    {
        if (!playerDead)
            this.gameObject.transform.localPosition = new Vector2(this.gameObject.transform.localPosition.x + speed * Time.deltaTime, this.gameObject.transform.localPosition.y);
    }
    public void OnStop(bool value)
    {
        playerDead = value;
    }
}
