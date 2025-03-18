using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class WaveEntry
{
    public List<GameObject> EnemyPrefab = new List<GameObject>();
    public List<int> EnemyCount = new List<int>();
}

[System.Serializable]
public class WaveData
{
    public int Wave;
    public List<WaveEntry> Enemies = new List<WaveEntry>();
}

public class WaveManager : MonoSingleton<WaveManager>
{
    public delegate void OnWaveEndEvent();
    public delegate void OnWaveChangeEvent(int wave);
    public OnWaveChangeEvent OnWaveChange;
    public OnWaveEndEvent OnWaveEnd;
    
    [Header("Wave Settings")]
    [SerializeField] private SOWaveData waveData;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private Transform[] spawnPoints;

    [Space(10)] [Header("Spawn Settings")] 
    [SerializeField] private VisualEffect monsterSpawnVFXPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;
    
    private bool _isSpawning = false;
    private bool _isSubWaveEnd = true;
    private int _currentWaveIndex = 0;
    private int _currentEntryIndex = 0;
    private float _spawnDelay;
    private List<Entity> _activeEnemies = new();
    private List<ItemSO> _gainedItemsList = new();
    
    public List<Entity> ActiveEnemies => _activeEnemies;
    public float CurrentTime { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public int CurrentWave => _currentWaveIndex + 1;
    
    private void Awake()
    {
        PlayerSpawn();
    }

    void Start()
    {
        _spawnDelay = monsterSpawnVFXPrefab.GetFloat("Delay");
        StartCoroutine(StartWaveRoutine());
    }

    public List<ItemSO> GetGainedItems()
    {
        return _gainedItemsList;
    }

    public void AddGainedItem(ItemSO item)
    {
        if (_gainedItemsList.Contains(item))
        {
            if (item.itemType == ItemType.Stuff)
            {
                int index = _gainedItemsList.IndexOf(item);
                _gainedItemsList[index].quantityOrLevel += item.quantityOrLevel;
            }
            else
            {
                _gainedItemsList.Add(item);
            }
        }
        _gainedItemsList.Add(item);
    }
    
    
    private IEnumerator StartWaveRoutine()
    {
        while (_currentWaveIndex < waveData.Waves.Count)
        {
            CurrentTime += Time.deltaTime;
            if (ActiveEnemies.Count == 0 && _isSubWaveEnd)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                _isSubWaveEnd = false;
                StartCoroutine(StartNewWave());
            }
            yield return null;
        }
        EndWave();
    }
    
    private IEnumerator StartNewWave()
    {
        OnWaveChange?.Invoke(CurrentWave);
        WaveData currentWave = waveData.Waves[_currentWaveIndex];
        _currentEntryIndex = 0;

        while (_currentEntryIndex < currentWave.Enemies.Count)
        {
            yield return new WaitForSeconds(1f);
            WaveEntry entry = currentWave.Enemies[_currentEntryIndex];
            SpawnEnemies(entry);
            
            Debug.Log($"Wave 시작 {_currentEntryIndex} ");
            
            // 다음 SubWave로 이동하기 전에 적이 전부 죽을 때까지 대기
            while (ActiveEnemies.Count > 0 || _isSpawning)
            {
                Debug.Log($"스폰이 되고 있는가 ? : {_isSpawning}");
                Debug.Log("SubWave 중");
                yield return null;
            }
            
            _currentEntryIndex++;
        }
        
        _isSubWaveEnd = true;
        _currentWaveIndex++;
    }

    private void SpawnEnemies(WaveEntry entry)
    {
        _isSpawning = true;
        for (int i = 0; i < entry.EnemyPrefab.Count; i++)
        {
            for (int j = 0; j < entry.EnemyCount[i]; j++)
            {
                int spawnIndex = j % spawnPoints.Length;
                StartCoroutine(SpawnDelay(entry.EnemyPrefab[i], spawnIndex));
            }
        }
    }

    private IEnumerator SpawnDelay(GameObject enemyPrefab, int spawnIndex)
    {
        var spawnEffect = Instantiate(monsterSpawnVFXPrefab, spawnPoints[spawnIndex].position, new Quaternion(0f, 90f, 0f, 0f));
        yield return new WaitForSeconds(_spawnDelay);
        
        Entity enemyInstance = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation).GetComponentInChildren<Entity>();
        ActiveEnemies.Add(enemyInstance);
        enemyInstance.onDead += RemoveEnemy;
        
        yield return new WaitForSeconds(1f);
        spawnEffect.Stop();
        _isSpawning = false;

        while (spawnEffect.aliveParticleCount > 0)
        {
            yield return null;
        }
        Destroy(spawnEffect.gameObject);
        
    }

    private void PlayerSpawn()
    { 
        PlayerTransform = Instantiate(playerPrefab, playerSpawnPoint).transform;
    }

    private void RemoveEnemy(Entity entity)
    {
        int removedCount = ActiveEnemies.RemoveAll(e => e.IsDead);
        Debug.Log($"{removedCount}명의 사망한 적 제거됨. 남은 적 수: {ActiveEnemies.Count}");
    }
    
    private void EndWave()
    {
        _currentWaveIndex = 0;
        OnWaveEnd?.Invoke();
        Time.timeScale = 0f;
    }
}