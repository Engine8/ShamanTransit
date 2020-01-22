using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    public Transform player;
    //public Chunk[] chunkPrefabs;
    public Chunk FirstChunk;

    private List<Chunk> spawnedChunks = new List<Chunk>();

    void Start()
    {
        spawnedChunks.Add(FirstChunk);
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
        Chunk newChunk = Instantiate(FirstChunk);
        newChunk.transform.position = new Vector2(spawnedChunks[spawnedChunks.Count - 1].endChunk.position.x - newChunk.beginChunk.localPosition.x, FirstChunk.transform.position.y) ;
        spawnedChunks.Add(newChunk);
        if (spawnedChunks.Count >= 5)
        {
            Destroy(spawnedChunks[0].gameObject);
            spawnedChunks.RemoveAt(0);
        }
    }
}
