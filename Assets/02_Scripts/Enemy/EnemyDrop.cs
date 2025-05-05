using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private DropTable dropTable;

    private Entity _owner;
    
    private void Start()
    {
        _owner =  GetComponent<Entity>();
        _owner.onDead += DropLoot;
    }

    public void DropLoot(Entity entity)
    {
        foreach (var drop in dropTable.drops)
        {
            float dropChance;
            switch (drop.gainType)
            {
                case GainType.Heart:
                    dropChance = _owner.Stats.HeartDropRate.Value;
                    break;
                default:
                    dropChance = drop.dropChance;
                    break;
            }
            
            if (Random.value <= dropChance) // 확률 체크
            {
                SpawnLoot(drop);
            }
        }
    }

    private void SpawnLoot(DropTable.DropEntry item)
    {
        Vector3 dropPosition = transform.position + new Vector3(Random.Range(-1f, 1f), 0.5f, Random.Range(-1f, 1f));
        GainLoot loot = Instantiate(item.lootPrefab, dropPosition, Quaternion.identity).GetComponent<GainLoot>();
        loot.Setup(_owner, item);
    }
}
