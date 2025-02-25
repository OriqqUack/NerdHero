using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Wave Data_", menuName = "Wave/WaveData", order = int.MaxValue)]
public class SOWaveData : ScriptableObject
{
    public List<WaveData> Waves = new List<WaveData>();
}
