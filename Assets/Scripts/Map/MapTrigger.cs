using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Enemy = 0,
        BossStart = 1,
        BossEndSection = 2,
    }

    public TriggerType Type;
    public GameObject EnemyPrefab;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ChunksPlacer.Instance.EnterTrigger(this);
    }

}
