using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

[System.Serializable]
public class WaveData
{
    public int Wave;
    public List<GameObject> EnemyPrefab;
    public List<int> EnemyCount;
}

public class WaveManager : MonoSingleton<WaveManager>
{
    [Header("Wave Settings")]
    [SerializeField] private SOWaveData wave;
    [SerializeField] private float timeBetweenWaves = 5f; // 웨이브 간 대기 시간
    [SerializeField] private Transform[] spawnPoints; // 적 소환 위치

    [Space(10)] [Header("Spawn Settings")] 
    [SerializeField] private VisualEffect spawnVFXPrefab;
    private float _spawnDelay;
    
    private List<Entity> _activeEnemies = new List<Entity>(); // 현재 살아있는 적 리스트
    private bool _isSpawning = false;
    private int _currentWave = 0; // 현재 웨이브
    private Dictionary<int, Dictionary<Entity, int>> wavesEnemies = new(); // 현재 Wave의 Entity Count
    
    public List<Entity> ActiveEnemies { get { return _activeEnemies; } }
    public int CurrentWave { get { return _currentWave; } }
    
    void Start()
    {
        Init();
        StartCoroutine(StartWaveRoutine());
    }

    private void Init()
    {
        //Wave Setting
        int j = 0;
        foreach (WaveData wave in wave.Waves)
        {
            int waveCount = wave.Wave;
            Dictionary<Entity, int> tempDic = new();
            for (int i = 0; i < wave.EnemyPrefab.Count; i++)
            {
                Entity entity = wave.EnemyPrefab[i].GetComponent<Entity>();
                int count = wave.EnemyCount[i];
                tempDic.Add(entity, count);
            }
            wavesEnemies.Add(waveCount, tempDic);
        }
        
        //Spawn Setting
        _spawnDelay = spawnVFXPrefab.GetFloat("Delay");
    }
    
    private IEnumerator StartWaveRoutine()
    {
        while (wavesEnemies.Count > _currentWave)
        {
            if (ActiveEnemies.Count == 0 && !_isSpawning)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                _isSpawning = true;
                StartNewWave(++_currentWave);
            }
            yield return null;
        }
    }
    
    private void StartNewWave(int currentWave)
    {
        var dic = wavesEnemies[currentWave];
        
        foreach (KeyValuePair<Entity, int> kvp in dic)
        {
            for (int j = 0; j < kvp.Value; j++) // 적 개수만큼 생성
            {
                int spawnIndex = j % spawnPoints.Length; // 스폰 위치 순환
                StartCoroutine(SpawnDelay(kvp, spawnIndex));
            }
        }
        
        _currentWave = currentWave;
    }

    private IEnumerator SpawnDelay(KeyValuePair<Entity, int> kvp, int spawnIndex)
    {
        var spawnEffect = Instantiate(spawnVFXPrefab, spawnPoints[spawnIndex].position, Quaternion.identity);
        yield return new WaitForSeconds(_spawnDelay);
        
        Entity enemyInstance = Instantiate(kvp.Key, spawnPoints[spawnIndex].position, Quaternion.identity);
        ActiveEnemies.Add(enemyInstance);
        enemyInstance.onDead += RemoveEnemy;

        yield return new WaitForSeconds(1f);
        spawnEffect.Stop();

        while (true)
        {
            if (spawnEffect.aliveParticleCount == 0)
            {
                Destroy(spawnEffect.gameObject);
                break;
            }
            yield return null;
        }
        
        _isSpawning = false;
    }

    private void RemoveEnemy(Entity entity)
    {
        ActiveEnemies.Remove(entity);
    }
}
