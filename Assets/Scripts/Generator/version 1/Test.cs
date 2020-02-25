using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            ChunksPlacer.Instance.OnAttack(transform.parent.gameObject.GetComponent<Tile>().EnemyPrefabs);
    }

}
