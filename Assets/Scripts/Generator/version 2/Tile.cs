﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile : MonoBehaviour
{
    public Transform Begin;
    public Transform End;
    public GameObject SpriteTile;
    public GameObject SpriteCliff;
    public GameObject EnemyPrefabs;
    public Land[] Lands;

    public List<Tile> NextTiles;
}
