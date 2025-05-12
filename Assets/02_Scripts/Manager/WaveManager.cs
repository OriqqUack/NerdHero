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
    public List<int> EnemyLevel = new List<int>();
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

    [Header("Wave Settings")] [SerializeField]
    private SOWaveData waveData;

    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private Transform[] spawnPoints;

    [Space(10)] [Header("Spawn Settings")] 
    //[SerializeField] private VisualEffect monsterSpawnVFXPrefab;

    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject confettiSpawnVFXPrefab;

    private bool _isSpawning = false;
    private bool _isSubWaveEnd = true;
    private bool _spawnedInBetweenWave;
    private int _currentWaveIndex = 0;
    private int _currentEntryIndex = 0;
    private int _spawningCount = 0;
    private int _spawnedEnemies = 0;
    private float _spawnDelay = 2f;
    private List<Entity> _activeEnemies = new();
    private List<ItemSO> _gainedItemsList = new();

    public List<Entity> ActiveEnemies => _activeEnemies;
    public float CurrentTime { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public Entity PlayerEntity { get; private set; }
    public int CurrentWave => _currentWaveIndex + 1;
    public int TotalWaveCount => waveData.Waves.Count;
    public bool IsClear { get; private set; }

    private void Awake()
    {
        IsClear = false; 
        
        if (playerPrefab)
            PlayerSpawn();
        else
            PlayerTransform = GameObject.FindWithTag("Player").transform;

        PlayerEntity = PlayerTransform.GetComponent<Entity>();
        
        if(GameManager.Instance.WaveData)
            waveData = GameManager.Instance.WaveData;
    }

    void Start()
    {
        //_spawnDelay = monsterSpawnVFXPrefab.GetFloat("Delay");
        StartCoroutine(StartWaveRoutine());
    }

    private void Update()
    {
        CurrentTime += Time.deltaTime;
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

    public void AddActiveEnemies(Entity entity)
    {
        ActiveEnemies.Add(entity);
        entity.onDead += RemoveEnemy;
        
        if(ActiveEnemies.Count == 0)
            _spawnedInBetweenWave = true;
    }


    private IEnumerator StartWaveRoutine()
    {
        while (_currentWaveIndex < waveData.Waves.Count)
        {
            // 1. 웨이브 시작 알림 먼저
            OnWaveChange?.Invoke(_currentWaveIndex + 1); // UI용으로는 1부터 시작한다고 가정

            // 2. StartNewWave 전에 인덱스 증가
            yield return StartCoroutine(StartNewWave(_currentWaveIndex));

            yield return new WaitUntil(() => ActiveEnemies.Count == 0 && _isSubWaveEnd);

            _currentWaveIndex++; // 루프 마지막에 증가
        }

        EndWave();
    }


    private IEnumerator StartNewWave(int waveIndex)
    {
        _currentEntryIndex = 0;

        if (!waveData.Waves.IsValidIndex(waveIndex))
        {
            Debug.LogWarning($"Wave index {waveIndex} is invalid.");
            yield break;
        }

        WaveData currentWave = waveData.Waves[waveIndex];

        while (_currentEntryIndex < currentWave.Enemies.Count)
        {
            WaveEntry entry = currentWave.Enemies[_currentEntryIndex];
            _isSubWaveEnd = false;

            SpawnEnemies(entry);

            yield return new WaitUntil(() => _isSubWaveEnd);
            _currentEntryIndex++;
        }
    }


    private void SpawnEnemies(WaveEntry entry)
    {
        for (int i = 0; i < entry.EnemyPrefab.Count; i++)
        {
            for (int j = 0; j < entry.EnemyCount[i]; j++)
            {
                int spawnIndex = _spawnedEnemies % spawnPoints.Length;
                _spawningCount++;
                _spawnedEnemies++;
                StartCoroutine(SpawnDelay(entry.EnemyPrefab[i], entry.EnemyLevel[i],spawnIndex));
            }
        }
    }

    private IEnumerator SpawnDelay(GameObject enemyPrefab, int level, int spawnIndex)
    {
        /*var spawnEffect = Instantiate(monsterSpawnVFXPrefab);
        spawnEffect.transform.position = spawnPoints[spawnIndex].position;*/
        yield return new WaitForSeconds(_spawnDelay);

        Entity enemyInstance =
            Instantiate(enemyPrefab, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation)
                .GetComponentInChildren<Entity>();
        enemyInstance.Stats.LevelSetup(level);
        enemyInstance.GetComponent<BehaviorTree>().enabled = false;
        enemyInstance.Animator.PlayOneShot("appear", 0, 0,
            () => enemyInstance.GetComponent<BehaviorTree>().enabled = true);

        ActiveEnemies.Add(enemyInstance);
        OnMonsterSpawn?.Invoke(ActiveEnemies);
        enemyInstance.onDead += RemoveEnemy;

        yield return new WaitForSeconds(1f);
        //spawnEffect.Stop();

        _spawningCount--;
        _isSpawning = _spawningCount > 0;
        
        /*while (spawnEffect.aliveParticleCount > 0) yield return null;
        Destroy(spawnEffect.gameObject);*/
    }

    private void PlayerSpawn()
    {
        PlayerTransform = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation).transform;
        PlayerEntity = PlayerTransform.GetComponent<Entity>();
    }

    public void RemoveEnemy(Entity entity)
    {
        ActiveEnemies.Remove(entity);
        Debug.Log($"{entity.name} 제거됨. 남은 적 수: {ActiveEnemies.Count}");

        if (ActiveEnemies.Count == 0 && !_isSpawning)
        {
            StartCoroutine(HandleSubWaveEnd());
        }
    }
    private IEnumerator HandleSubWaveEnd()
    {
        float elapsedTime = 0;
    
        while (elapsedTime < timeBetweenWaves)
        {
            if (_spawnedInBetweenWave)
            {
                _spawnedInBetweenWave = false;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isSubWaveEnd = true;
    }

    private void EndWave()
    {
        StartCoroutine(CountAndEnd());
    }

    IEnumerator CountAndEnd()
    {
        yield return new WaitForSeconds(1f);

        GameManager.Instance.IsClear = true;
        PlayerEntity.Movement.Stop();
        PlayerEntity.Animator.PlayAnimationForState("idle", 0);
        PlayerEntity.Animator.PlayAnimationForState("clear emotion", 2);
        PlayerEntity.Animator.PlayOneShot("jump", 0);
        Instantiate(confettiSpawnVFXPrefab, PlayerTransform.position, Quaternion.identity);

        yield return new WaitForSeconds(2f);

        OnWaveEnd?.Invoke();
        
        Time.timeScale = 0f;
    }
}