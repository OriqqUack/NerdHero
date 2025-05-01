using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LightningDealDamageAction : ElementEffectAction
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
    [SerializeField]
    private float bonusDamagePerLevel;
    [SerializeField]
    private float bonusDamagePerStack;
    [SerializeField]
    private float bounceRange = 5f; // 전이 범위
    [SerializeField]
    private int maxBounceTargets = 2; // 최대 전이 수
    [SerializeField]
    private float bounceDamageFactor = 0.7f; // 전이 피해 배율
    
    private List<Entity> _allEntities = new List<Entity>();

    private float GetDefaultDamage(Effect effect)
        => defaultDamage + (effect.DataBonusLevel * bonusDamagePerLevel);

    private float GetStackDamage(int stack)
        => (stack - 1) * bonusDamagePerStack;

    private float GetBonusStatDamage(Entity user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;

    private float GetTotalDamage(Effect effect, Entity user, int stack, float scale)
    {
        // Damage 계산 공식
        // (defaultValue + (bonusLevel * bonusDamageByLevel)) + ((stack - 1) * bonusDamageByStack) + (bonusDamageStat.Value * bonuDamageStatFactor);
        var totalDamage = GetDefaultDamage(effect) + GetStackDamage(stack);
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

        // 주변 적에게 번개 전이
        var bounceTargets = FindNearbyEnemies(user, target, bounceRange, maxBounceTargets);
        foreach (var bounceTarget in bounceTargets)
        {
            float bounceDamage = totalDamage * bounceDamageFactor;
            bounceTarget.TakeDamage(user, effect, bounceDamage);

            // 이펙트가 있다면 여기에 전이 이펙트도 연출 가능
            // e.g., PlayLightningEffect(target, bounceTarget);
        }

        return true;
    }
    
    private List<Entity> FindNearbyEnemies(Entity origin, Entity excludeTarget, float range, int maxCount)
    {
        var allEntities = WaveManager.Instance.ActiveEnemies;
        var nearby = new List<Entity>();

        foreach (var e in allEntities)
        {
            if (e == excludeTarget || e == origin || e == null)
                continue;

            float dist = Vector3.Distance(origin.transform.position, e.transform.position);
            if (dist <= range)
                nearby.Add(e);
        }

        nearby.Sort((a, b) =>
            Vector3.Distance(origin.transform.position, a.transform.position)
                .CompareTo(Vector3.Distance(origin.transform.position, b.transform.position)));

        return nearby.GetRange(0, Mathf.Min(maxCount, nearby.Count));
    }

    protected override IReadOnlyDictionary<string, string> GetStringsByKeyword(Effect effect)
    {
        var descriptionValuesByKeyword = new Dictionary<string, string>
        {
            ["defaultDamage"] = GetDefaultDamage(effect).ToString(".##"),
            ["bonusDamageStat"] = bonusDamageStat?.DisplayName ?? string.Empty,
            ["bonusDamageStatFactor"] = (bonusDamageStatFactor * 100f).ToString() + "%",
            ["bonusDamagePerLevel"] = bonusDamagePerLevel.ToString(),
            ["bonusDamagePerStack"] = bonusDamagePerStack.ToString(),
        };

        if (effect.User)
        {
            descriptionValuesByKeyword["totalDamage"] =
                GetTotalDamage(effect, effect.User, effect.CurrentStack, effect.Scale).ToString(".##");
        }
         
        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new LightningDealDamageAction()
        {
            defaultDamage = defaultDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor,
            bonusDamagePerLevel = bonusDamagePerLevel,
            bonusDamagePerStack = bonusDamagePerStack
        };
    }
}
