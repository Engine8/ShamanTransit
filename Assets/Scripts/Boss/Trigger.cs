using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        { 
            GameObject newEnemy = Instantiate(transform.parent.gameObject.GetComponent<Tile>().EnemyPrefabs);
            GameController.Instance.SetGameMode(1, true);
        }
    }
}
