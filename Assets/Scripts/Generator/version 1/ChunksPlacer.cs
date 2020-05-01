using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunksPlacer : MonoBehaviour
{
    public static ChunksPlacer Instance;

    public PlayerController Player;

    public HitArea HitAreaRef;
    public Map[] Maps;
    public Map EndlessMap;
    public int MapIndex;
    public GameObject UIAttack;

    public Tile ChunkPrefabsAttack;

    private bool _isAttackTileSpawned = false;
    private int _indexChunk = 0;
    private string _holderName = "Generated Map";
    private Transform _mapHolder;
    private Map _currentMap;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        if (GameData.Instance != null)
        {
            MapIndex = GameData.Instance.CurrentLevelIndex;
        }

        if (GameData.Instance.CurrentPlayMode == PlayMode.Standart)
            _currentMap = Maps[MapIndex];
        else if (GameData.Instance.CurrentPlayMode == PlayMode.Endless)
            _currentMap = EndlessMap;
    }

    void Start()
    {
        _mapHolder = new GameObject(_holderName).transform;
        _mapHolder.parent = transform;
        _currentMap.spawnedChunks.Add(Instantiate(_currentMap.TilePrefabsTurn[_indexChunk], new Vector3(0, 0, 0), Quaternion.identity));
        _currentMap.spawnedChunks[0].gameObject.transform.parent = _mapHolder;
        ++_indexChunk;
        SpawnChunk();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player.transform.position.x > _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].End.position.x - 14)
        {
            SpawnChunk();
            //SpawnChunk();
        }
    }

    public void OnAttack(GameObject enemyPrefab)
    {
        GameController.Instance.SetGameMode(1, true);
        GameObject newEnemy = Instantiate(enemyPrefab);

        EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
        newEnemy.transform.position = new Vector2(Player.transform.position.x - 18, -1.81f);

        SoundManager.Instance.PlaySoundClip(enemyController.EnterSound, true);
        if (enemyController.IsCameraShaking)
            GameController.Instance.ShakeCamera(enemyController.EnterSound.length);

        HitAreaRef.SetEnnemy(enemyController);
    }

    private void SpawnChunk()
    {
        Tile newChunk = null;
        if (!GameController.Instance.IsAttackMode) 
        {
            if (GameController.Instance.CurrentPlayMode == PlayMode.Standart)
                if (_indexChunk < _currentMap.TilePrefabsTurn.Length)
                {
                    newChunk = Instantiate(_currentMap.TilePrefabsTurn[_indexChunk]);
                    ++_indexChunk;
                }
            else
                {
                    Tile lastTile = _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1];
                    int nextTileIndex = Random.Range(0, lastTile.NextTiles.Count);
                    newChunk = Instantiate(lastTile.NextTiles[nextTileIndex]);
                }
        }
        else
        {
            Tile lastAttackTile;
            //define last attack tile or take first prefab
            if (_isAttackTileSpawned)
                lastAttackTile = _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1];
            else
                lastAttackTile = ChunkPrefabsAttack;
            //random define next tile
            int nextTileIndex = Random.Range(0, lastAttackTile.NextTiles.Count);
            newChunk = Instantiate(lastAttackTile.NextTiles[nextTileIndex]);
            _isAttackTileSpawned = true;
        }

        if (newChunk == null)
            return;

        newChunk.transform.position = new Vector2(_currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].End.position.x - newChunk.Begin.localPosition.x, 0);
        _currentMap.spawnedChunks.Add(newChunk);
        _currentMap.spawnedChunks[_currentMap.spawnedChunks.Count - 1].gameObject.transform.parent = _mapHolder;
        if (_currentMap.spawnedChunks.Count >= 5)
        {
            Destroy(_currentMap.spawnedChunks[0].gameObject);
            _currentMap.spawnedChunks.RemoveAt(0);
        }
    }



    public int GetMoneyMultiplier()
    {
        return _currentMap.MoneyMultiplier;
    }

    public void GetSnowInfo(out bool isSnowy, out WindStatus windStatus)
    {
        isSnowy = _currentMap.IsSnowy;
        windStatus = _currentMap.WindStatusVar;
    }

    [System.Serializable]
    public class Map
    {
        public int MoneyMultiplier;
        public bool IsSnowy;
        public WindStatus WindStatusVar;
        public Tile[] TilePrefabsTurn;
        [HideInInspector]
        public List<Tile> spawnedChunks = new List<Tile>();
    }
 }

public enum WindStatus
{
    Not = 0,
    TriggerOnly = 1,
    Random = 2
}
