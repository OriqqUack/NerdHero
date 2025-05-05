using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatGrowthData
{
    public int Level;
    public float MaxHp;
    public float Damage;
    public int expChargeAmount;
    public float heartDropRate;
    public int energyChargeAmount;
}

[CreateAssetMenu(fileName = "StatGrowthTable_", menuName = "GameData/StatGrowthTable")]
public class StatGrowthTable : ScriptableObject
{
    public List<StatGrowthData> GrowthDataList;
}