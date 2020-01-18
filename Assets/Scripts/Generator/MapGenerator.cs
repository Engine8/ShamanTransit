using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Tile TilePrefab;
    public int LengthMap;
    public GameObject[] BlocksPrefabs;
    public TileBlock[] TileBlocks;

    public bool IsNeedRegenerateMap = false;

    private List<Tile> _spawnedTiles = new List<Tile>();

    private string _holderName = "Generator Map";

    private void Start()
    {
        //tileBlocks = new TileBlock[lengthMap];
        //GanarateMap();
    }

    public void TryGenerateMap()
    {
        if (IsNeedRegenerateMap)
        {
            SpawnMap();
            IsNeedRegenerateMap = false;
        }
        else
        {
            GenerateMap();
        }
    }

    //Delete previous generated map and creates new clear <LengthMap> tiles
    public void SpawnMap()
    {
        if (transform.Find(_holderName))
        {
            DestroyImmediate(transform.Find(_holderName).gameObject);
            _spawnedTiles.Clear();
        }
        TileBlock.LengthLand = TilePrefab.Lands.Length;
        TileBlocks = new TileBlock[LengthMap];
        
        Transform mapHolder = new GameObject(_holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < LengthMap; ++x)
        {
            if (x == 0)
            {
                _spawnedTiles.Add(Instantiate(TilePrefab, new Vector3(0, 0, 0), Quaternion.identity));
                _spawnedTiles[0].gameObject.transform.parent = mapHolder;
            }
            else
            {
                Tile newTile = Instantiate(TilePrefab);
                newTile.transform.position = _spawnedTiles[_spawnedTiles.Count - 1].End.position - newTile.Begin.localPosition;
                _spawnedTiles.Add(newTile);
                newTile.gameObject.transform.parent = mapHolder;
            }
        }
    }

    //handle the blocks editing in editor 
    public void GenerateMap()
    {
        for (int tileIndex = 0; tileIndex < LengthMap; ++tileIndex)
        {
            Debug.Log($"TileIndex: {tileIndex}/{_spawnedTiles.Count}");
            for (int landIndex = 0; landIndex < TileBlocks[tileIndex].LandsBlocks.Length; ++landIndex)
            {
                for (int blockIndex = 0; blockIndex < TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks.Length; ++blockIndex)
                {
                    if (!TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].Active && _spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex] != null)
                    {
                        _spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex].SetActive(TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].Active);
                    }
                    else
                    {
                        _spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex].SetActive(TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].Active);
                        switch (TileBlocks[tileIndex].LandsBlocks[landIndex].Blocks[blockIndex].BlockType)
                        {
                            case 0:
                                CreateBlock(_spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex], 0);
                                break;
                            case 1:
                                CreateBlock(_spawnedTiles[tileIndex].Lands[landIndex].SpawnedBlocks[blockIndex], 1);
                                break;
                        }
                    }
                }
            }
        }
    }

    //place/replace block by given prefab
    private void CreateBlock(GameObject Block, int prefabIndex)
    {
        DestroyImmediate(Block.transform.GetChild(0).gameObject);
        GameObject newBlock = Instantiate(BlocksPrefabs[prefabIndex]);
        newBlock.transform.parent = Block.transform;
        newBlock.transform.localPosition = new Vector3(0, 1, -0.1f);
    }

    [System.Serializable]
    public class TileBlock
    {
        public static int LengthLand;

        public LandsBlock[] LandsBlocks = new LandsBlock[LengthLand];

        [System.Serializable]
        public class LandsBlock
        {
            public Block[] Blocks = new Block[LengthLand];
            [System.Serializable]
            public class Block
            {
                [Range(0, 1)]
                public int BlockType;
                public bool Active;
            }
        }
    }

}
