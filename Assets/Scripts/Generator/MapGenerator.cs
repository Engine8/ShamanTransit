using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Tile tilePrefab;
    public int lengthMap;

    private List<Tile> spawnedTile = new List<Tile>();
    private void Start()
    {
        GenarateMap();
    }
    public void GenarateMap()
    {
        string holderName = "Generator Map";
        if (transform.FindChild(holderName))
        {
            DestroyImmediate(transform.FindChild(holderName).gameObject);
            spawnedTile.Clear();
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < lengthMap; ++x) 
        {   
            if (x == 0)
            {
                spawnedTile.Add(Instantiate(tilePrefab, new Vector3(0, 0, 0), Quaternion.identity));
                spawnedTile[0].gameObject.transform.parent = mapHolder;
            }
            else
            {
                Tile newTile = Instantiate(tilePrefab);
                newTile.transform.position = spawnedTile[spawnedTile.Count - 1].End.position - newTile.Begin.localPosition;
                spawnedTile.Add(newTile);
                newTile.gameObject.transform.parent = mapHolder;
            }
        }
    }

}
