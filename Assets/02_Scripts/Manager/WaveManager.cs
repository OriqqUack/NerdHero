// 개선된 WaveManager: Coroutine 최소화 버전
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class WaveEntry {
    public List<GameObject> EnemyPrefab = new List<GameObject>();
    public List<int> EnemyCount = new List<int>();
}

[System.Serializable]
public class WaveData {
    public int Wave;
    public List<WaveEntry> Enemies = new List<WaveEntry>();
}

public class WaveManager : MonoSingleton<WaveManager> {
    public delegate void OnWaveEndEvent();
    public delegate void OnWaveChangeEvent(int wave);
    public OnWaveChangeEvent OnWaveChange;
    public OnWaveEndEvent OnWaveEnd;

    [Header("Wave Settings")]
    [SerializeField] private SOWaveData waveData;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Spawn Settings")]
    //[SerializeField] private VisualEffect monsterSpawnVFXPrefab;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject confettiSpawnVFXPrefab;

    private int _currentWaveIndex = 0;
    private int _currentEntryIndex = 0;
    private int _spawnCount = 0;

    private float _spawnDelay;
    private float _spawnTimer = 0f;
    private bool _spawningWave = false;
    private bool _isEnding = false;

    private Queue<(GameObject, Vector3)> spawnQueue = new();
    private List<Entity> _activeEnemies = new();
    private List<ItemSO> _gainedItemsList = new();

    public List<Entity> ActiveEnemies => _activeEnemies;
    public float CurrentTime { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public int CurrentWave => _currentWaveIndex + 1;

    private void Awake() {
        PlayerSpawn();
    }

    void Start() {
        //_spawnDelay = monsterSpawnVFXPrefab.GetFloat("Delay");
        Invoke(nameof(BeginWaveCheck), 1f);
    }

    void Update() {
        CurrentTime += Time.deltaTime;

        if (_isEnding) return;

        if (_spawningWave && spawnQueue.Count > 0) {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _spawnDelay) {
                _spawnTimer = 0f;
                var (prefab, pos) = spawnQueue.Dequeue();

                //SpawnEffect(pos);
                GameObject go = Instantiate(prefab, pos, prefab.transform.rotation);
                Entity enemy = go.GetComponentInChildren<Entity>();
                _activeEnemies.Add(enemy);
                enemy.onDead += RemoveEnemy;
            }
        }

        // 웨이브 시작 조건
        if (!_spawningWave && ActiveEnemies.Count == 0) {
            if (_currentWaveIndex < waveData.Waves.Count) {
                _spawningWave = true;
                StartNewWave();
            } else {
                EndWave();
            }
        }
    }

    void BeginWaveCheck() {
        _spawningWave = true;
    }

    private void StartNewWave() {
        OnWaveChange?.Invoke(CurrentWave);
        WaveData currentWave = waveData.Waves[_currentWaveIndex];
        _currentEntryIndex = 0;
        QueueNextSubwave(currentWave);
    }

    private void QueueNextSubwave(WaveData wave) {
        if (_currentEntryIndex >= wave.Enemies.Count) {
            _currentWaveIndex++;
            _spawningWave = false;
            return;
        }

        WaveEntry entry = wave.Enemies[_currentEntryIndex];
        for (int i = 0; i < entry.EnemyPrefab.Count; i++) {
            for (int j = 0; j < entry.EnemyCount[i]; j++) {
                int spawnIndex = _spawnCount % spawnPoints.Length;
                _spawnCount++;
                Vector3 spawnPos = spawnPoints[spawnIndex].position;
                spawnQueue.Enqueue((entry.EnemyPrefab[i], spawnPos));
            }
        }
        _currentEntryIndex++;
    }

    /*private void SpawnEffect(Vector3 position) {
        var vfx = Instantiate(monsterSpawnVFXPrefab, position, monsterSpawnVFXPrefab.transform.rotation);
        vfx.Play();
        Destroy(vfx.gameObject, 3f); // 타이머 기반 제거 (성능 개선)
    }*/

    private void PlayerSpawn() {
        PlayerTransform = Instantiate(playerPrefab, playerSpawnPoint).transform;
    }

    private void RemoveEnemy(Entity entity) {
        _activeEnemies.Remove(entity);

        if (_spawningWave && _activeEnemies.Count == 0 && spawnQueue.Count == 0) {
            WaveData currentWave = waveData.Waves[_currentWaveIndex];
            QueueNextSubwave(currentWave);
        }
    }

    private void EndWave()
    {
        _isEnding = true;
        StartCoroutine(CountAndEnd());
    }

    IEnumerator CountAndEnd() 
    {
        Entity entity = PlayerTransform.GetComponent<Entity>();
        entity.Animator.PlayAnimationForState("clear emotion", 2);
        var track = entity.Animator.PlayOneShot("jump", 0);
        Instantiate(confettiSpawnVFXPrefab, entity.transform.position, Quaternion.identity);
        track.End += entry => entity.Movement.enabled = true;
        entity.Movement.enabled = false;

        yield return new WaitForSeconds(5f);
        OnWaveEnd?.Invoke();
        Time.timeScale = 0f;
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
}
