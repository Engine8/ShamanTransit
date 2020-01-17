using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Tile tilePrefab;
    public int lengthMap;
    public GameObject[] blockPrefabs;
    public TileBlock[] tileBlocks;


    private List<Tile> spawnedTile = new List<Tile>();

    private void Start()
    {
        tileBlocks = new TileBlock[lengthMap];
        //GanarateMap();
    }
    public void GanarateMap()
    {
        string holderName = "Generator Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
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
            for (int landIndex = 0; landIndex < tileBlocks[x].landsBlocks.Length; ++landIndex)
            {
                for (int blockIndex = 0; blockIndex < tileBlocks[x].landsBlocks[landIndex].Blocks.Length; ++blockIndex)
                {
                    if (!tileBlocks[x].landsBlocks[landIndex].Blocks[blockIndex].activ && spawnedTile[x].Lands[landIndex].spawnBlock[blockIndex] != null)
                    {
                        spawnedTile[x].Lands[landIndex].spawnBlock[blockIndex].SetActive(tileBlocks[x].landsBlocks[landIndex].Blocks[blockIndex].activ);
                    }
                    else
                    {
                        spawnedTile[x].Lands[landIndex].spawnBlock[blockIndex].SetActive(tileBlocks[x].landsBlocks[landIndex].Blocks[blockIndex].activ);
                        switch (tileBlocks[x].landsBlocks[landIndex].Blocks[blockIndex].typeBlock)
                        {
                            case 0:
                                CreateBloc(spawnedTile[x].Lands[landIndex].spawnBlock[blockIndex], 0);
                                break;
                            case 1:
                                CreateBloc(spawnedTile[x].Lands[landIndex].spawnBlock[blockIndex], 1);
                                break;
                        }
                    }
                }
            }
        }
    }
    private void CreateBloc(GameObject Block, int indexPrefabs)
    {
        DestroyImmediate(Block.transform.GetChild(0).gameObject);
        GameObject newBlock = Instantiate(blockPrefabs[indexPrefabs]);
        newBlock.transform.parent = Block.transform;
        newBlock.transform.localPosition = new Vector3(0, 1, -0.1f);
    }

    [System.Serializable]
    public class TileBlock
    {
        public static int lengthLand;

        public LandsBlock[] landsBlocks = new LandsBlock[lengthLand];

        [System.Serializable]
        public class LandsBlock
        {
            public Block[] Blocks = new Block[lengthLand];
            [System.Serializable]
            public class Block
            {
                [Range(0, 1)]
                public int typeBlock;
                public bool activ;
            }
        }
    }

}
