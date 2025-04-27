using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using DG.Tweening;
using Pathfinding;
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

public static class ListExtensions
{
    public static bool IsValidIndex<T>(this List<T> list, int index)
    {
        return index >= 0 && index < list.Count;
    }
}

public class WaveManager : MonoSingleton<WaveManager>
{
    public delegate void OnWaveEndEvent();
    public delegate void OnWaveChangeEvent(int wave);
    public delegate void OnMonsterSpawnEvent(List<Entity> entities);
    public OnWaveChangeEvent OnWaveChange;
    public OnWaveEndEvent OnWaveEnd;
    public OnMonsterSpawnEvent OnMonsterSpawn;
    
    [Header("Wave Settings")]
    [SerializeField] private SOWaveData waveData;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private Transform[] spawnPoints;

    [Space(10)] [Header("Spawn Settings")] 
    [SerializeField] private VisualEffect monsterSpawnVFXPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject confettiSpawnVFXPrefab;
    
    private List<Entity> _activeEnemies = new();
    private List<ItemSO> _gainedItemsList = new();

    private int _currentWaveIndex;
    private int _currentSubWaveIndex;
    private int _spawnedEnemies;
    private bool _isSubWaveRunning;
    private bool _isSpawning;
    private float _subWaveTimer;
    private float _spawnDelay;
    private float _waveDuration = 10f;
    private float _subWaveDuration = 5f;

    private Coroutine _subWaveCo;
    
    public List<Entity> ActiveEnemies => _activeEnemies;
    public float CurrentTime { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public Entity PlayerEntity { get; private set; }
    public int CurrentWave => _currentWaveIndex;
    public int TotalWaveCount => waveData.Waves.Count;
    public float RemainingWaveTime
    {
        get
        {
            if (_isSubWaveRunning)
                return Mathf.Clamp01(_subWaveTimer / _subWaveDuration);
            else
                return Mathf.Clamp01(_subWaveTimer / _waveDuration);
        }
    }
    private void Awake()
    {
        if (playerPrefab)
            PlayerSpawn();
        else
            PlayerTransform = GameObject.FindWithTag("Player").transform;

        PlayerEntity = PlayerTransform.GetComponent<Entity>();
        
        waveData = GameManager.Instance.WaveData;
    }

    private void Start()
    {
        _spawnDelay = monsterSpawnVFXPrefab.GetFloat("Delay");
        StartCoroutine(WaveRoutine());
    }

    public List<ItemSO> GetGainedItems()
    {
        return _gainedItemsList;
    }
    
    public void AddGainedItem(ItemSO item)
    {
        int index = _gainedItemsList.IndexOf(item);
        if (index >= 0 && item.itemType == ItemType.Stuff)
        {
            _gainedItemsList[index].quantityOrLevel += item.quantityOrLevel;
        }
        else
        {
            _gainedItemsList.Add(item);
        }
    }
    
    private IEnumerator WaveRoutine()
    {
        while (_currentWaveIndex < waveData.Waves.Count)
        {
            yield return new WaitUntil(() => _activeEnemies.Count == 0 || !_isSubWaveRunning);
            yield return new WaitForSeconds(timeBetweenWaves);
            yield return StartCoroutine(StartWave());
        }
    }

    private IEnumerator StartWave()
    {
        _currentSubWaveIndex = 0;
        _currentWaveIndex++;
        OnWaveChange?.Invoke(CurrentWave);

        _subWaveTimer = _waveDuration;
        
        while (_currentSubWaveIndex < waveData.Waves[_currentWaveIndex - 1].Enemies.Count)
        {
            if(_subWaveCo != null)
                StopCoroutine(_subWaveCo);
            _subWaveCo = StartCoroutine(StartSubWave());
            _isSpawning = true;
            
            yield return new WaitUntil(() => _subWaveTimer <= 0 );
            yield return new WaitForSeconds(1f);
            _currentSubWaveIndex++;
        }

        _isSubWaveRunning = false;
    }

    private IEnumerator StartSubWave()
    {
        if (waveData.Waves[_currentWaveIndex - 1].Enemies.IsValidIndex(_currentSubWaveIndex + 1))
        {
            _subWaveTimer = _subWaveDuration;
        }
        else
        {
            _subWaveTimer = _waveDuration;
        }

        SpawnEnemies(waveData.Waves[_currentWaveIndex - 1].Enemies[_currentSubWaveIndex]);

        while (_subWaveTimer > 0f)
        {
            _subWaveTimer -= Time.deltaTime;
            yield return null;
        }
    }

    private void SpawnEnemies(WaveEntry entry)
    {
        for (int i = 0; i < entry.EnemyPrefab.Count; i++)
        {
            for (int j = 0; j < entry.EnemyCount[i]; j++)
            {
                int spawnIndex = _spawnedEnemies % spawnPoints.Length;
                _spawnedEnemies++;
                StartCoroutine(SpawnDelay(entry.EnemyPrefab[i], spawnIndex));
            }
        }
    }

    private IEnumerator SpawnDelay(GameObject enemyPrefab, int spawnIndex)
    {
        var spawnEffect = Instantiate(monsterSpawnVFXPrefab);
        spawnEffect.transform.position = spawnPoints[spawnIndex].position;
        yield return new WaitForSeconds(_spawnDelay);

        Entity enemy = Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation).GetComponentInChildren<Entity>();
        enemy.GetComponent<BehaviorTree>().enabled = false;
        enemy.Animator.PlayOneShot("appear", 0, 0, () => enemy.GetComponent<BehaviorTree>().enabled = true);

        _activeEnemies.Add(enemy);
        enemy.onDead += RemoveEnemy;
        OnMonsterSpawn?.Invoke(_activeEnemies);

        yield return new WaitForSeconds(1f);
        spawnEffect.Stop();
        while (spawnEffect.aliveParticleCount > 0) yield return null;
        Destroy(spawnEffect.gameObject);
        
        _isSpawning = false;
    }

    private void RemoveEnemy(Entity entity)
    {
        _activeEnemies.Remove(entity);
        if (_activeEnemies.Count == 0)
        {
            _subWaveTimer = 0f;
        }
        if (_currentWaveIndex == TotalWaveCount && _activeEnemies.Count == 0)
        {
            EndAllWaves();
        }
    }

    private void PlayerSpawn()
    {
        PlayerTransform = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation).transform;
        PlayerEntity = PlayerTransform.GetComponent<Entity>();
    }

    private void EndAllWaves()
    {
        StartCoroutine(EndRoutine());
    }

    private IEnumerator EndRoutine()
    {
        PlayerEntity.Movement.isCC = true;
        PlayerEntity.Animator.PlayAnimationForState("idle", 0);
        PlayerEntity.Animator.PlayAnimationForState("clear emotion", 2);
        PlayerEntity.Animator.PlayOneShot("jump", 0);
        Instantiate(confettiSpawnVFXPrefab, PlayerTransform.position, Quaternion.identity);

        yield return new WaitForSeconds(5f);

        OnWaveEnd?.Invoke();
        Time.timeScale = 0f;
    }
}