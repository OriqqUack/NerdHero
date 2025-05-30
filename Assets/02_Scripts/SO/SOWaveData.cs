using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Wave Data_", menuName = "Wave/WaveData", order = int.MaxValue)]
public class SOWaveData : ScriptableObject
{
    public List<WaveData> Waves = new List<WaveData>();
}

[System.Serializable]
public class WaveData
{
    public int Wave;
    public List<WaveEntry> Enemies = new List<WaveEntry>();
}

[System.Serializable]
public class WaveEntry
{
    public List<int> EnemyLevel = new List<int>();
    public List<GameObject> EnemyPrefab = new List<GameObject>();
    public List<int> EnemyCount = new List<int>();
}



