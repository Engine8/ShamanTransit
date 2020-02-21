using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject.FindObjectOfType<ChunksPlacer>().OnAttack(transform.parent.gameObject.GetComponent<Tile>().EnemyPrefabs);
    }

}
