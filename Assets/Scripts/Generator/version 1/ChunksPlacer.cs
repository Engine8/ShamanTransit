using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    public Transform player;

    public Map[] map;
    public int mapIndex;
    public Tile[] chunkPrefabs;
    public GameObject UIAttack;
    public Tile chunkPrefabsAttack;

    private bool stateAttack;
    private int indexChunk = 0;
    string holderName = "Generated Map";
    Transform mapHolder;

    Map currentMap;
    void Start()
    {
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        currentMap = map[mapIndex];
        currentMap.spawnedChunks.Add(Instantiate(currentMap.TilePrefabsTurn[indexChunk], new Vector3(0, 0, 0), Quaternion.identity));
        currentMap.spawnedChunks[0].gameObject.transform.parent = mapHolder;
        ++indexChunk;
        SpawnChunk();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > currentMap.spawnedChunks[currentMap.spawnedChunks.Count - 1].End.position.x - 14)
        {
            SpawnChunk();
            SpawnChunk();
        }
    }
    public void OnAttack(GameObject enemy)
    {
        stateAttack = true;
        UIAttack.SetActive(true);
        GameObject newChunk = Instantiate(enemy);

        FindObjectOfType<HitArea>().SetEnnemy(newChunk.GetComponent<EnemyController>());
        Debug.Log(newChunk.GetComponent<EnemyController>().GetCount());
        newChunk.transform.position = new Vector2(player.position.x - 20, -1.27f);
    }
    public void Victiry()
    {
        stateAttack = false;
    }
    private void SpawnChunk()
    {
        if (!stateAttack) 
        {
            if (indexChunk< currentMap.TilePrefabsTurn.Length) {
                Tile newChunk = Instantiate(currentMap.TilePrefabsTurn[indexChunk]);
                newChunk.transform.position = new Vector2(currentMap.spawnedChunks[currentMap.spawnedChunks.Count - 1].End.position.x - newChunk.Begin.localPosition.x, 0);
                currentMap.spawnedChunks.Add(newChunk);
                currentMap.spawnedChunks[currentMap.spawnedChunks.Count - 1].gameObject.transform.parent = mapHolder;
                if (currentMap.spawnedChunks.Count >= 5)
                {
                    Destroy(currentMap.spawnedChunks[0].gameObject);
                    currentMap.spawnedChunks.RemoveAt(0);
                }
                ++indexChunk; 
            }
        }
        else
        {
            Tile newChunk = Instantiate(chunkPrefabsAttack);
            newChunk.transform.position = new Vector2(currentMap.spawnedChunks[currentMap.spawnedChunks.Count - 1].End.position.x - newChunk.Begin.localPosition.x, 0);
            currentMap.spawnedChunks.Add(newChunk);
            currentMap.spawnedChunks[currentMap.spawnedChunks.Count - 1].gameObject.transform.parent = mapHolder;
            if (currentMap.spawnedChunks.Count >= 5)
            {
                Destroy(currentMap.spawnedChunks[0].gameObject);
                currentMap.spawnedChunks.RemoveAt(0);
            }
        }
    }
    [System.Serializable]
    public class Map
    {
        public Tile[] TilePrefabsTurn;
        [HideInInspector]
        public List<Tile> spawnedChunks = new List<Tile>();
    }
 }
