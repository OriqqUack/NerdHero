using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

[System.Serializable]
public class WaveEntry
{
    public GameObject EnemyPrefab;
    public int EnemyCount;
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
    private int _currentWaveIndex = 0;
    private int _currentEntryIndex = 0;
    private float _spawnDelay;
    private List<Entity> _activeEnemies = new();
    
    public List<Entity> ActiveEnemies => _activeEnemies;
    public float CurrentTime { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public int CurrentWave => _currentWaveIndex + 1;
    public Dictionary<Item, float> GainedItemsList { get; private set; } = new Dictionary<Item, float>();
    
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
            if (ActiveEnemies.Count == 0 && !_isSpawning)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                _isSpawning = true;
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
            WaveEntry entry = currentWave.Enemies[_currentEntryIndex];
            yield return StartCoroutine(SpawnEnemies(entry));
            _currentEntryIndex++;
        }
        
        _currentWaveIndex++;
        _isSpawning = false;
    }

    private IEnumerator SpawnEnemies(WaveEntry entry)
    {
        for (int i = 0; i < entry.EnemyCount; i++)
        {
            int spawnIndex = i % spawnPoints.Length;
            yield return SpawnDelay(entry.EnemyPrefab, spawnIndex);
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

