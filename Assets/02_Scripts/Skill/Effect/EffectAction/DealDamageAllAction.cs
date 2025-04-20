using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DealPerDamageAllAction : EffectAction
{
    [SerializeField]
    private Stat damageStat;
    [SerializeField]
    private float defaultPerDamage;
    [SerializeField]
    private Stat bonusDamageStat;
    // Bonus 값을 줄 Stat에 적용할 Factor
    // Stat이 주는 Bonus 값 = bonusDamageStat.Value * bonusDamageStatFactor
    [SerializeField]
    private float bonusDamageStatFactor;
    
    private float GetBonusStatDamage(Entity user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        float totalDamage = target.Stats.GetValue(damageStat) * defaultPerDamage;
        if (bonusDamageStat)
            totalDamage += GetBonusStatDamage(user);
        foreach (var entity in WaveManager.Instance.ActiveEnemies)
        {
            entity.TakeDamage(user, effect, totalDamage);
        }

        return true;
    }

    public override object Clone()
    {
        return new DealPerDamageAllAction()
        {
            defaultPerDamage = defaultPerDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor
        };
    }
}
