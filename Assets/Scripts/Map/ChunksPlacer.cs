using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    public static ChunksPlacer Instance;
    public PlayerController player;
    public Map[] map;
    public int mapIndex;
    public GameObject UIAttack;

    private bool stateAttack;
    private int indexChunk = 0;
    private string holderName = "Generated Map";
    private Transform _mapHolder;
    private Map _currentMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        /*
        if (GameData.Instance != null)
        {
            /////////////////////////mapIndex = GameData.Instance.CurrentLevelIndex;
            mapIndex = 1;
        }
        */
        _currentMap = map[1]; //map[mapIndex];
    }

    void Start()
    {
        _mapHolder = new GameObject(holderName).transform;
        _mapHolder.parent = transform;
        _currentMap.spawnedChunks.Add(Instantiate(_currentMap.TilePrefabsTurn[indexChunk], new Vector3(0, 0, 0), Quaternion.identity));
        _currentMap.spawnedChunks[0].gameObject.transform.parent = _mapHolder;
        ++indexChunk;
        SpawnChunk();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.x > _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].End.position.x - 12)
        {
            SpawnChunk();
        }
    }

    public void EnterTrigger(MapTrigger mapTrigger)
    {   

    }

    private void SpawnChunk()
    {
        if (GameController.Instance.CurrentGameStatus != GameController.GameStatus.Attack) 
        {
            if (indexChunk< _currentMap.TilePrefabsTurn.Length) {
                Tile newChunk = Instantiate(_currentMap.TilePrefabsTurn[indexChunk]);
                newChunk.transform.position = new Vector2(_currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].End.position.x - newChunk.Begin.localPosition.x, 0);
                _currentMap.spawnedChunks.Add(newChunk);
                _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].gameObject.transform.parent = _mapHolder;
                if (_currentMap.spawnedChunks.Count >= 5)
                {
                    Destroy(_currentMap.spawnedChunks[0].gameObject);
                    _currentMap.spawnedChunks.RemoveAt(0);
                }
                ++indexChunk;
            }
        }
        else
        {
            Tile newChunk = Instantiate(_currentMap.AttackTiles[Random.Range(0, _currentMap.AttackTiles.Count)]);
            newChunk.transform.position = new Vector2(_currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].End.position.x - newChunk.Begin.localPosition.x, 0);
            _currentMap.spawnedChunks.Add(newChunk);
            _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].gameObject.transform.parent = _mapHolder;
            if (_currentMap.spawnedChunks.Count >= 5)
            {
                Destroy(_currentMap.spawnedChunks[0].gameObject);
                _currentMap.spawnedChunks.RemoveAt(0);
            }
        }
    }

    public int GetMoneyMultiplier()
    {
        return _currentMap.MoneyMultiplier;
    }

    public int GetBasicReward()
    {
        return _currentMap.BasicReward;
    }

    public void GetSnowInfo(out bool isSnowy, out WindStatus windStatus)
    {
        isSnowy = _currentMap.IsSnowy;
        windStatus = _currentMap.WindStatusVar;
    }

    public GameMode GetMapGameMode()
    {
        return _currentMap.MapMode;
    }

    public AudioClip GetAudio()
    {
        return _currentMap.LevelMusic;
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
        public List<Tile> AttackTiles;
        public AudioClip LevelMusic;
    }
 }

public enum WindStatus
{
    Not = 0,
    TriggerOnly = 1,
    Random = 2
}
