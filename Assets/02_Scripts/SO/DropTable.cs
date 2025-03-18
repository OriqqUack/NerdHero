using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "SO/DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        public GainType gainType; // 스탯 or 아이템 획득 타입 설정
        public Stat stat;
        public float statAmount;
        public GameObject lootPrefab;
        public ItemSO item;
        [Range(0f, 1f)] public float dropChance; // 0.0 ~ 1.0 확률
    }

    public List<DropEntry> drops = new List<DropEntry>();
}
