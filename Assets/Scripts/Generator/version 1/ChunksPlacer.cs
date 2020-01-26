using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    public Transform player;
    //public Chunk[] chunkPrefabs;
    public Chunk FirstChunk;
    public Chunk ChunkPrefabs;

    private List<Chunk> spawnedChunks = new List<Chunk>();
    private float positionYFirstChunk;
    void Start()
    {
        spawnedChunks.Add(FirstChunk);
        positionYFirstChunk = FirstChunk.transform.position.y;
        transform.gameObject.layer = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > spawnedChunks[spawnedChunks.Count - 1].endChunk.position.x -8)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    {
     // Chunk newChunk = Instantiate(chunkPrefabs[Random.Range(0, chunkPrefabs.Length)]);
        Chunk newChunk = Instantiate(ChunkPrefabs);
        newChunk.transform.position = new Vector2(spawnedChunks[spawnedChunks.Count - 1].endChunk.position.x - newChunk.beginChunk.localPosition.x, positionYFirstChunk) ;
        spawnedChunks.Add(newChunk);
        if (spawnedChunks.Count >= 5)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
    }
}
