using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardProbabilityConfig
{
    public int level;
    public float commonRate;
    public float rareRate;
    public float legendaryRate;
}

public class CardProbabilityManager
{
    public List<CardProbabilityConfig> gradeProbConfigs;

    private Dictionary<AttributeType, float> GetAttributeRates(AttributeType recent, AttributeType previous)
    {
        var rates = new Dictionary<AttributeType, float>();

        foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
        {
            rates[attr] = 20f;
        }

        if (recent == previous)
        {
            rates[recent] = 25f;
            float remainRate = (100f - 25f) / 4f;
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
            {
                if (attr != recent) rates[attr] = remainRate;
            }
        }
        else
        {
            rates[recent] = 25f;
            rates[previous] = 20f;
            float remainRate = (100f - 25f - 20f) / 3f;
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
            {
                if (attr != recent && attr != previous) rates[attr] = remainRate;
            }
        }

        return rates;
    }

    public (EffectRarity grade, AttributeType attr)? RollCard(
        int level,
        AttributeType recent,
        AttributeType previous,
        HashSet<AttributeType> selectedForLegendary)
    {
        var gradeConfig = gradeProbConfigs.Find(c => c.level == level);
        if (gradeConfig == null) return null;

        // 1. 등급 결정
        float r = UnityEngine.Random.value * 100f;
        EffectRarity selectedGrade;
        if (r < gradeConfig.commonRate) selectedGrade = EffectRarity.Common;
        else if (r < gradeConfig.commonRate + gradeConfig.rareRate) selectedGrade = EffectRarity.Rare;
        else selectedGrade = EffectRarity.Legendary;

        // 2. 속성 확률 결정
        var attrRates = GetAttributeRates(recent, previous);
        float total = 0f;
        foreach (var rate in attrRates.Values) total += rate;
        float pick = UnityEngine.Random.value * total;
        float cumulative = 0f;
        AttributeType selectedAttr = AttributeType.BaseAttack;

        foreach (var kvp in attrRates)
        {
            cumulative += kvp.Value;
            if (pick <= cumulative)
            {
                selectedAttr = kvp.Key;
                break;
            }
        }

        // 3. 전설 카드 예외 처리: 속성 카테고리에 대해서만 필터 적용
        if (selectedGrade == EffectRarity.Legendary && selectedAttr == AttributeType.Element)
        {
            if (!selectedForLegendary.Contains(selectedAttr))
            {
                return null; // 속성 카테고리인데 일반/레어 선택 이력 없음 → 등장 불가
            }
        }

        return (selectedGrade, selectedAttr);
    }
}