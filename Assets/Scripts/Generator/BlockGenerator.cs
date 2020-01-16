using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    public Block[] Blocks;
    public GameObject[] BlockPrefabs;

    private void Start()
    {
        GeneratorBlock();
    }

   public void GeneratorBlock()
    {
      
        for (int i = 0; i < Blocks.Length; ++i) 
        {
            if (!Blocks[i].IsActive && Blocks[i].SpawnBlock != null)
            {
                Blocks[i].SpawnBlock.SetActive(Blocks[i].IsActive);
            }
            else 
            {
                Blocks[i].SpawnBlock.SetActive(Blocks[i].IsActive);
                switch (Blocks[i].TypeBlock)
                {
                    case 0:
                        CreateBloc(i, 0);
                        break;
                    case 1:
                        CreateBloc(i, 1);
                        break;
                } 
            }
        }
    }

    private void CreateBloc(int indexBlock, int indexPrefabs)
    {
        DestroyImmediate(Blocks[indexBlock].SpawnBlock.transform.GetChild(0).gameObject);
        GameObject newBlock = Instantiate(BlockPrefabs[indexPrefabs]);
        newBlock.transform.parent = Blocks[indexBlock].SpawnBlock.transform;
        newBlock.transform.localPosition = new Vector3(0, 0, -0.1f);
    }

     [System.Serializable]
    public class Block
    {
        [Range(0,1)]
        public int TypeBlock;
        public bool IsActive;
        public GameObject SpawnBlock;

    }
}

