using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    public static ChunksPlacer Instance;

    public PlayerController player;

    public HitArea HitAreaRef;
    public Map[] map;
    public int mapIndex;
    //public Tile[] chunkPrefabs;
    public GameObject UIAttack;
    public Tile chunkPrefabsAttack;

    private bool stateAttack;
    private int indexChunk = 0;
    string holderName = "Generated Map";
    Transform mapHolder;

    Map currentMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (GameData.Instance != null)
        {
            mapIndex = GameData.Instance.CurrentLevelIndex;
        }
        currentMap = map[mapIndex];
    }

    void Start()
    {
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;
        //currentMap = map[mapIndex];
        currentMap.spawnedChunks.Add(Instantiate(currentMap.TilePrefabsTurn[indexChunk], new Vector3(0, 0, 0), Quaternion.identity));
        currentMap.spawnedChunks[0].gameObject.transform.parent = mapHolder;
        ++indexChunk;
        SpawnChunk();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > currentMap.spawnedChunks[currentMap.spawnedChunks.Count - 1].End.position.x - 12)
        {
            SpawnChunk();
        }
    }

    public void EnterTrigger(MapTrigger mapTrigger)
    {   
        if (mapTrigger.Type == MapTrigger.TriggerType.Enemy || mapTrigger.Type == MapTrigger.TriggerType.BossStart)
        {
            GameObject newEnemy = Instantiate(mapTrigger.EnemyPrefab);
            newEnemy.transform.position = new Vector2(player.transform.position.x - 18, -1.81f);

            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            SoundManager.Instance.PlaySoundClip(enemyController.EnterSound, true);
            if (enemyController.IsCameraShaking)
                GameController.Instance.ShakeCamera(enemyController.EnterSound.length);
            HitAreaRef.SetEnemy(enemyController);
            if (mapTrigger.Type == MapTrigger.TriggerType.Enemy)
            {
                GameController.Instance.SetGameStatus(GameController.GameStatus.Attack, true);
            }
            else
            {
                GameController.Instance.SetGameStatus(GameController.GameStatus.BossRun, true);
            }
        }
        else if (mapTrigger.Type == MapTrigger.TriggerType.BossEndSection) //start the attack phase
        {
            HitAreaRef.BossBattleSectionStart();
        }
    }

    private void SpawnChunk()
    {
        if (GameController.Instance.CurrentGameStatus != GameController.GameStatus.Attack) 
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

    public int GetMoneyMultiplier()
    {
        return currentMap.MoneyMultiplier;
    }

    public int GetBasicReward()
    {
        return currentMap.BasicReward;
    }

    public void GetSnowInfo(out bool isSnowy, out WindStatus windStatus)
    {
        isSnowy = currentMap.IsSnowy;
        windStatus = currentMap.WindStatusVar;
    }

    public GameMode GetMapGameMode()
    {
        return currentMap.MapMode;
    }

    [System.Serializable]
    public class Map
    {
        public int MoneyMultiplier;
        public int BasicReward;
        public bool IsSnowy;
        public WindStatus WindStatusVar;
        public Tile[] TilePrefabsTurn;
        [HideInInspector]
        public List<Tile> spawnedChunks = new List<Tile>();
        public GameMode MapMode;
    }
 }

public enum WindStatus
{
    Not = 0,
    TriggerOnly = 1,
    Random = 2
}
