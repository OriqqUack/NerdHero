using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public enum WaveState
{
    Idle,
    Preparing,
    Spawning,
    WaitingForClear,
    Completed
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
    public delegate void OnWaveStartEvent(int waveIndex);
    public delegate void OnWaveEndEvent();
    public delegate void OnEnemySpawnedEvent(List<Entity> entities);

    public OnWaveStartEvent OnWaveStart;
    public OnWaveEndEvent OnWaveEnd;
    public OnEnemySpawnedEvent OnEnemySpawned;

    [Header("Wave Settings")]
    [SerializeField] private SOWaveData waveData;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float timeBetweenWaves = 3f;
    [SerializeField] private bool useRandomSpawn = true;

    [Header("Player Settings")]
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject confettiSpawnVFXPrefab;

    public Transform PlayerTransform { get; private set; }
    public Entity PlayerEntity { get; private set; }
    public float CurrentTime { get; private set; }

    private Queue<SpawnInfo> spawnQueue = new Queue<SpawnInfo>();
    public List<Entity> ActiveEnemies = new List<Entity>();
    public List<ItemSO> GainedItems = new List<ItemSO>();

    private int currentWaveIndex = 0;
    private WaveState currentState = WaveState.Idle;
    private float stateTimer = 0f;
    private int sequentialSpawnIndex = 0;
    private List<int> shuffledSpawnIndices = new List<int>();
    private int shuffledIndexPointer = 0;
    private QuestReporter questReporter;

    void Awake()
    {
        currentState = WaveState.Preparing;

        if (playerPrefab)
            PlayerSpawn();
        else
            PlayerTransform = GameObject.FindWithTag("Player").transform;

        PlayerEntity = PlayerTransform.GetComponent<Entity>();

        if(GameManager.Instance.WaveData)
            waveData = GameManager.Instance.WaveData;
        
        questReporter = GetComponent<QuestReporter>();
    }

    void Update()
    {
        CurrentTime += Time.deltaTime;
        
        Debug.Log(ActiveEnemies.Count + " Enemies Spawned");

        switch (currentState)
        {
            case WaveState.Preparing:
                PrepareWave();
                break;

            case WaveState.Spawning:
                if (spawnQueue.Count > 0)
                {
                    stateTimer -= Time.deltaTime;
                    if (stateTimer <= 0f)
                    {
                        SpawnNext();
                        stateTimer = spawnDelay;
                    }
                }
                else if (ActiveEnemies.Count > 0)
                {
                    currentState = WaveState.WaitingForClear;
                }
                else
                {
                    ProceedToNextWave();
                }
                break;

            case WaveState.WaitingForClear:
                if (ActiveEnemies.Count == 0)
                {
                    ProceedToNextWave();
                }
                break;
        }
    }

    public void AddEnemies(Entity entity)
    {
        ActiveEnemies.Add(entity);
        entity.onDead += RemoveEnemy;
    }

    void PrepareWave()
    {
        if (currentWaveIndex >= waveData.Waves.Count)
        {
            currentState = WaveState.Completed;
            GameManager.instance.IsClear = true;
            OnWaveEnd?.Invoke();
            return;
        }

        WaveData wave = waveData.Waves[currentWaveIndex];
        spawnQueue.Clear();
        if(useRandomSpawn)
            sequentialSpawnIndex = 0;
        InitShuffledSpawnIndices();

        foreach (var entry in wave.Enemies)
        {
            for (int i = 0; i < entry.EnemyPrefab.Count; i++)
            {
                for (int j = 0; j < entry.EnemyCount[i]; j++)
                {
                    spawnQueue.Enqueue(new SpawnInfo(entry.EnemyPrefab[i], entry.EnemyLevel[i]));
                }
            }
        }

        OnWaveStart?.Invoke(currentWaveIndex + 1);
        currentState = WaveState.Spawning;
        stateTimer = spawnDelay;
    }

    void InitShuffledSpawnIndices()
    {
        shuffledSpawnIndices.Clear();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            shuffledSpawnIndices.Add(i);
        }
        Shuffle(shuffledSpawnIndices);
        shuffledIndexPointer = 0;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }

    void SpawnNext()
    {
        if (spawnQueue.Count == 0) return;

        var spawnInfo = spawnQueue.Dequeue();
        int index;

        if (useRandomSpawn)
        {
            if (shuffledIndexPointer >= shuffledSpawnIndices.Count)
            {
                InitShuffledSpawnIndices();
            }
            index = shuffledSpawnIndices[shuffledIndexPointer];
            shuffledIndexPointer++;
        }
        else
        {
            index = sequentialSpawnIndex % spawnPoints.Length;
            sequentialSpawnIndex++;
        }

        Entity enemyInstance =
            Instantiate(spawnInfo.prefab, spawnPoints[index].position, spawnPoints[index].rotation)
                .GetComponentInChildren<Entity>();
        enemyInstance.Stats.LevelSetup(spawnInfo.level);
        enemyInstance.GetComponent<BehaviorTree>().enabled = false;
        enemyInstance.Animator.PlayOneShot("appear", 0, 0,
            () => enemyInstance.GetComponent<BehaviorTree>().enabled = true);
        enemyInstance.onDead += RemoveEnemy;

        ActiveEnemies.Add(enemyInstance);
        OnEnemySpawned?.Invoke(ActiveEnemies);
    }

    public void RemoveEnemy(Entity entity)
    {
        ActiveEnemies.Remove(entity);
        questReporter?.Report();
    }

    void ProceedToNextWave()
    {
        currentWaveIndex++;
        StartCoroutine(NextWaveDelay());
    }

    IEnumerator NextWaveDelay()
    {
        currentState = WaveState.Idle;
        yield return new WaitForSeconds(timeBetweenWaves);
        currentState = WaveState.Preparing;
    }

    struct SpawnInfo
    {
        public GameObject prefab;
        public int level;

        public SpawnInfo(GameObject prefab, int level)
        {
            this.prefab = prefab;
            this.level = level;
        }
    }

    public List<Entity> GetActiveEnemies() => ActiveEnemies;
    public int CurrentWave => currentWaveIndex + 1;
    public int TotalWaveCount => waveData.Waves.Count;
    public bool IsWaveCompleted => currentState == WaveState.Completed;

    private void PlayerSpawn()
    {
        PlayerTransform = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation).transform;
        PlayerEntity = PlayerTransform.GetComponent<Entity>();
    }
}
