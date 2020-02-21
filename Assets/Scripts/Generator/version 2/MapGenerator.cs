using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] map;
    public int mapIndex;
    public Tile TilePrefab;


    public GameObject[] BlocksPrefabs;
    public GameObject[] EnemyPrefabs;

    string holderName = "Generated Map";
    Map currentMap;

    public void TryGenerateMap()
    {
        currentMap = map[mapIndex];
        SpawnMap();
        GenerateMap();
    }

    //Delete previous generated map and creates new clear <LengthMap> tiles
    public void SpawnMap()
    {

        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
            currentMap._spawnedTiles.Clear();
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < currentMap.LengthMap; ++x)
        {
            if (x == 0)
            {
                currentMap._spawnedTiles.Add(Instantiate(TilePrefab, new Vector3(0, 0, 0), Quaternion.identity));
                currentMap._spawnedTiles[0].gameObject.transform.parent = mapHolder;
            }
            else
            {
                Tile newTile = Instantiate(TilePrefab);
                newTile.transform.position = currentMap._spawnedTiles[currentMap._spawnedTiles.Count - 1].End.position - newTile.Begin.localPosition;
                currentMap._spawnedTiles.Add(newTile);
                newTile.gameObject.transform.parent = mapHolder;
            }
        }
    }

    //handle the blocks editing in editor 
    public void GenerateMap()
    {
        for (int tileIndex = 0; tileIndex < currentMap.LengthMap; ++tileIndex)
        {
            if (!currentMap.TileBlocks[tileIndex].cliff && !currentMap.TileBlocks[tileIndex].TrigerEnemy)
            {
                currentMap._spawnedTiles[tileIndex].SpriteTile.SetActive(true);
                currentMap._spawnedTiles[tileIndex].SpriteCliff.SetActive(false);

                for (int landIndex = 0; landIndex < currentMap.TileBlocks[tileIndex].LandsBlocks.Length; ++landIndex)
                {

                    for (int blockIndex = 0; blockIndex < currentMap.TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks.Length; ++blockIndex)
                    {
                        if (!currentMap.TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].Active && currentMap._spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex] != null)
                        {
                            currentMap._spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex].SetActive(currentMap.TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].Active);
                        }
                        else
                        {
                            currentMap._spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex].SetActive(currentMap.TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].Active);
                            CreateBlock(currentMap._spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex], currentMap.TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].BlockType, landIndex);
                        }
                    }
                }
            }
            else if (currentMap.TileBlocks[tileIndex].cliff && !currentMap.TileBlocks[tileIndex].TrigerEnemy)
            {
                currentMap._spawnedTiles[tileIndex].SpriteTile.SetActive(false);
                currentMap._spawnedTiles[tileIndex].SpriteCliff.SetActive(true);
            }
            else
            {
                currentMap._spawnedTiles[tileIndex].EnemyPrefabs = EnemyPrefabs[currentMap.TileBlocks[tileIndex].EnemyType];
            }
        }
    }

    //place/replace block by given prefab
    private void CreateBlock(GameObject Block, int prefabIndex, int lineIndex)
    {
        DestroyImmediate(Block.transform.GetChild(0).gameObject);
        GameObject newBlock = Instantiate(BlocksPrefabs[prefabIndex]);
        newBlock.transform.parent = Block.transform;
        newBlock.transform.localPosition = new Vector3(0, 1, -0.1f);

        //set physics and graphics layers
        //Line 1 - 8 physcics layer - Line1 grapichs
        //Line 2 - 9 physcics layer - Line2 grapichs
        //Line 3 - 10 physcics layer - Line3 grapichs
        if (lineIndex == 0)
        {
            newBlock.layer = 8;
            newBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Line1";
        }
        else if (lineIndex == 1)
        {
            newBlock.layer = 9;
            newBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Line2";
        }
        else if (lineIndex == 2)
        {
            newBlock.layer = 10;
            newBlock.GetComponent<SpriteRenderer>().sortingLayerName = "Line3";
        }
    }
    [System.Serializable]
    public class Map
    {
        public int LengthMap;
        public TileBlock[] TileBlocks;


        [HideInInspector]
        public List<Tile> _spawnedTiles = new List<Tile>();
        [HideInInspector]
        public bool _needToFindTiles = true;


        [System.Serializable]
        public class TileBlock
        {
            public static int LengthLand;
            public bool cliff;//обрыв
            public bool TrigerEnemy;
            [Range(0, 1)]
            public int EnemyType;
            public LandsBlock[] LandsBlocks = new LandsBlock[LengthLand];


            [System.Serializable]
            public class LandsBlock
            {
                public Block[] Blocks = new Block[LengthLand];
                [System.Serializable]
                public class Block
                {
                    [Range(0, 4)]
                    public int BlockType;
                    public bool Active;
                }
            }
        }
    }
}
