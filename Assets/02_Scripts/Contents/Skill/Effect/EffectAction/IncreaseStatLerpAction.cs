using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class IncreaseStatLerpAction : EffectAction
{
    [SerializeField]
    private Stat stat;
    [SerializeField] 
    private bool isPercent;
    [SerializeField]
    private float defaultValue;
    [SerializeField]
    private Stat bonusValueStat;
    [SerializeField]
    private float bonusValueStatFactor;
    [SerializeField]
    private float bonusValuePerLevel;
    [SerializeField]
    private float bonusValuePerStack;
    // ������ ���� Stat�� DefaultValue�� ���� ���ΰ�? Bonus Value�� �߰��� ���ΰ�?
    [SerializeField]
    private bool isBonusType = true;
    // ������ ���� Release�� �� �ǵ��� ���ΰ�?
    [SerializeField]
    private bool isUndoOnRelease = true;

    private float totalValue;
    private Coroutine statChangeCoroutine;
    private float GetDefaultValue(Effect effect)
    {
        return defaultValue + (effect.DataBonusLevel * bonusValuePerLevel);
    }

    private float GetStackValue(int stack)
        => (stack - 1) * bonusValuePerStack;

    private float GetBonusStatValue(Entity user)
        => user.Stats.GetValue(bonusValueStat) * bonusValueStatFactor;

    private float GetTotalValue(Effect effect, Entity user, int stack, float scale)
    {
        float defaultValue;
        if (isPercent)
            defaultValue = GetDefaultValue(effect) * user.Stats.GetValue(stat);
        else
            defaultValue = GetDefaultValue(effect);
        
        totalValue = defaultValue + GetStackValue(stack);
        if (bonusValueStat)
            totalValue += GetBonusStatValue(user);

        totalValue *= scale;

        return totalValue;
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        totalValue = GetTotalValue(effect, user, stack, scale);

        float currentValue = isBonusType
            ? target.Stats.GetBonusValue(stat)
            : target.Stats.GetValue(stat); // 또는 적절한 현재값

        if (statChangeCoroutine != null)
            target.StopCoroutine(statChangeCoroutine);

        statChangeCoroutine = target.StartCoroutine(
            ApplyStatOverTime(target, currentValue, currentValue + totalValue, 1.0f, isBonusType) // duration 1초 예시
        );

        return true;
    }


    public override void Release(Effect effect, Entity user, Entity target, int level, float scale)
    {
        if (!isUndoOnRelease)
            return;

        if (isBonusType)
            target.Stats.RemoveBonusValue(stat, this);
        else
            target.Stats.IncreaseDefaultValue(stat, -totalValue);
    }

    private IEnumerator ApplyStatOverTime(Entity target, float from, float to, float duration, bool isBonus)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float current = Mathf.Lerp(from, to, elapsed / duration);

            if (isBonus)
                target.Stats.SetBonusValue(stat, this, current);
            else
                target.Stats.IncreaseDefaultValue(stat, current - from); // delta 적용

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 최종 값 보정
        if (isBonus)
            target.Stats.SetBonusValue(stat, this, to);
        else
            target.Stats.IncreaseDefaultValue(stat, to - from);
    }
    
    public override void OnEffectStackChanged(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        if (!isBonusType)
            Release(effect, user, target, level, scale);

        Apply(effect, user, target, level, stack, scale);
    }

    protected override IReadOnlyDictionary<string, string> GetStringsByKeyword(Effect effect)
    {
        var descriptionValuesByKeyword = new Dictionary<string, string>
        {
            { "stat", stat.DisplayName },
            { "defaultValue", GetDefaultValue(effect).ToString("0.##") },
            { "bonusDamageStat", bonusValueStat?.DisplayName ?? string.Empty },
            { "bonusDamageStatFactor", (bonusValueStatFactor * 100f).ToString() + "%" },
            { "bonusDamageByLevel", bonusValuePerLevel.ToString() },
            { "bonusDamageByStack", bonusValuePerStack.ToString() },
        };

        if (effect.Owner != null)
        {
            descriptionValuesByKeyword.Add("totalValue",
                GetTotalValue(effect, effect.User, effect.CurrentStack, effect.Scale).ToString("0.##"));
        }

        return descriptionValuesByKeyword;

    }

    public override object Clone()
    {
        return new IncreaseStatLerpAction()
        {
            stat = stat,
            isPercent = isPercent,
            defaultValue = defaultValue,
            bonusValueStat = bonusValueStat,
            bonusValueStatFactor = bonusValueStatFactor,
            bonusValuePerLevel = bonusValuePerLevel,
            bonusValuePerStack = bonusValuePerStack,
            isBonusType = isBonusType,
            isUndoOnRelease = isUndoOnRelease
        };
    }
}
