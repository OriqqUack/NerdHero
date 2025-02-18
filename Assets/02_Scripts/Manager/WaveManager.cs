using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class WaveData
{
    public int Wave;
    public List<string> EnemyName = new List<string>();
    public List<int> EnemyCount = new List<int>();
}

public class WaveManager : MonoSingleton<WaveManager>
{
    [Header("Wave Settings")]
    public List<WaveData> waves = new List<WaveData>();
    [SerializeField] private float timeBetweenWaves = 5f; // 웨이브 간 대기 시간
    [SerializeField] private Transform[] spawnPoints; // 적 소환 위치

    private List<Entity> _activeEnemies = new List<Entity>(); // 현재 살아있는 적 리스트
    private bool _isWaveActive = false;
    private int _currentWave = 0; // 현재 웨이브
    private Dictionary<int, Dictionary<Entity, int>> wavesEnemies = new();
    
    public List<Entity> ActiveEnemies { get { return _activeEnemies; } }
    public int CurrentWave { get { return _currentWave; } }
    
    void Start()
    {
        Init();
        StartCoroutine(StartWaveRoutine());
    }

    private void Init()
    {
        foreach (WaveData wave in waves)
        {
            int waveCount = wave.Wave - 1;
            Dictionary<Entity, int> tempDic = new();
            for (int i = 0; i < wave.EnemyName.Count; i++)
            {
                Entity entity = Resources.Load<Entity>("Prefabs/Enemies/" + wave.EnemyName[i]);
                int count = wave.EnemyCount[i];
                tempDic.Add(entity, count);
            }
            wavesEnemies.Add(waveCount, tempDic);
        }
    }
    
    private IEnumerator StartWaveRoutine()
    {
        while (wavesEnemies.Count >= _currentWave)
        {
            if (ActiveEnemies.Count == 0)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
                StartNewWave(++_currentWave);
            }
            yield return null;
        }
    }
    
    private void StartNewWave(int currentWave)
    {
        var dic = wavesEnemies[currentWave - 1];
        foreach (KeyValuePair<Entity, int> kvp in dic)
        {
            for (int j = 0; j < kvp.Value; j++) // 적 개수만큼 생성
            {
                int spawnIndex = j % spawnPoints.Length; // 스폰 위치 순환
                Entity enemyInstance = Instantiate(kvp.Key, spawnPoints[spawnIndex].position, Quaternion.identity);
                ActiveEnemies.Add(enemyInstance);
                enemyInstance.onDead += RemoveEnemy;
            }
        }
        
        _currentWave = currentWave;
    }

    private void RemoveEnemy(Entity entity)
    {
        ActiveEnemies.Remove(entity);
        Destroy(entity);
    }
}
