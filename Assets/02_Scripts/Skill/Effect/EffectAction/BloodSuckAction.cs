using UnityEngine;

[System.Serializable]

public class BloodSuckAction : EffectAction
{
    [SerializeField]
    private float defaultDamage;
    // Bonus 값으로 줄 Stat
    [SerializeField]
    private Stat bonusDamageStat;
    // Bonus 값을 줄 Stat에 적용할 Factor
    // Stat이 주는 Bonus 값 = bonusDamageStat.Value * bonusDamageStatFactor
    [SerializeField]
    private float bonusDamageStatFactor;
    
    private float GetBonusStatDamage(Entity user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;
    
    private float GetTotalDamage(Effect effect, Entity user, int stack, float scale)
    {
        // Damage 계산 공식
        // (defaultValue + (bonusLevel * bonusDamageByLevel)) + ((stack - 1) * bonusDamageByStack) + (bonusDamageStat.Value * bonuDamageStatFactor);
        var totalDamage = defaultDamage;
        if (bonusDamageStat)
            totalDamage += GetBonusStatDamage(user);

        // 마지막으로 Effect의 Scale로 Damage를 Scaling함
        totalDamage *= scale;

        return totalDamage;
    }
    
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        var totalDamage = GetTotalDamage(effect, user, stack, scale);
        target.TakeDamage(user, effect, totalDamage);
        
        user.Stats.HPStat.DefaultValue += totalDamage;
        
        return true;
    }

    public override object Clone()
    {
        return new BloodSuckAction()
        {
            defaultDamage = defaultDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor
        };
    }
}
