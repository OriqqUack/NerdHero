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
    
    public List<Entity> ActiveEnemies => _activeEnemies;
    public float CurrentTime { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public int CurrentWave => _currentWaveIndex + 1;
    public Dictionary<ItemSO, float> GainedItemsList { get; private set; } = new Dictionary<ItemSO, float>();
    
    private void Awake()
    {
        PlayerSpawn();
    }

    void Start()
    {
        _spawnDelay = monsterSpawnVFXPrefab.GetFloat("Delay");
        StartCoroutine(StartWaveRoutine());
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
            
            // 다음 SubWave로 이동하기 전에 적이 전부 죽을 때까지 대기
            while (ActiveEnemies.Count > 0 || _isSpawning)
            {
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
        var spawnEffect = Instantiate(monsterSpawnVFXPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        yield return new WaitForSeconds(_spawnDelay);
        
        Entity enemyInstance = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, Quaternion.identity).GetComponent<Entity>();
        ActiveEnemies.Add(enemyInstance);
        enemyInstance.onDead += RemoveEnemy;
        
        yield return new WaitForSeconds(1f);
        spawnEffect.Stop();

        while (spawnEffect.aliveParticleCount > 0)
        {
            yield return null;
        }
        Destroy(spawnEffect.gameObject);
        
        _isSpawning = false;
    }

    private void PlayerSpawn()
    { 
        PlayerTransform = Instantiate(playerPrefab, playerSpawnPoint).transform;
    }

    private void RemoveEnemy(Entity entity)
    {
        ActiveEnemies.Remove(entity);
    }

    public void EndWave()
    {
        _currentWaveIndex = 0;
        OnWaveEnd?.Invoke();
        Time.timeScale = 0f;
    }
}