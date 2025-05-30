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
    private List<CardProbabilityConfig> _gradeProbConfigs;

    public CardProbabilityManager(List<CardProbabilityConfig> gradeProbConfigs)
    {
        this._gradeProbConfigs = gradeProbConfigs;
    }
    
    private Dictionary<AttributeType, float> GetAttributeRates(AttributeType recent, AttributeType previous, bool isFirst)
    {
        var rates = new Dictionary<AttributeType, float>();

        if (isFirst)
        {
            foreach (AttributeType attr in Enum.GetValues(typeof(AttributeType)))
                rates[attr] = 20f;

            rates[AttributeType.None] = 0f;
            return rates;
        }

        if (recent == previous)
        {
            rates[recent] = 25f;
            float remainRate = (100f - 25f) / 4f;
            Array values = Enum.GetValues(typeof(AttributeType));

            for (int i = 0; i < values.Length - 1; i++)
            {
                AttributeType attr = (AttributeType)values.GetValue(i);
                if (attr != recent)
                {
                    rates[attr] = remainRate;
                }
            }
        }
        else
        {
            Array values = Enum.GetValues(typeof(AttributeType));

            rates[recent] = 25f;
            if(previous != AttributeType.None)
                rates[previous] = 20f;
            float remainRate = (100f - 25f - rates[previous]) / (values.Length - 2);

            for (int i = 0; i < values.Length - 1; i++)
            {
                AttributeType attr = (AttributeType)values.GetValue(i);
                if (attr != recent && attr != previous) rates[attr] = remainRate;
            }
        }

        return rates;
    }

    public (EffectRarity grade, AttributeType attr)?  RollCard(
        int level,
        HashSet<AttributeType> commonRareHistory,
        AttributeType attributeCategory,
        AttributeType recent,
        AttributeType previous,
        bool isFirst)
    {
        var config = _gradeProbConfigs.Find(c => c.level == level);
        if (config == null) return null;

        // 1. 등급 결정
        float r = UnityEngine.Random.value * 100f;
        EffectRarity grade;
        if (r < config.commonRate) grade = EffectRarity.Common;
        else if (r < config.commonRate + config.rareRate) grade = EffectRarity.Rare;
        else grade = EffectRarity.Legendary;

        // 2. 속성 결정 - 확률 기반으로 선택
        var attrRates = GetAttributeRates(recent, previous, isFirst);
        AttributeType attr = WeightedRandom(attrRates);

        // 3. 전설 카드 예외 조건
        if (grade == EffectRarity.Legendary && attr == attributeCategory)
        {
            if (!commonRareHistory.Contains(attr))
            {
                Debug.Log("[RollCard] 전설 조건 불충족 - 속성 카테고리 전설은 동일 속성의 Common/Rare가 있어야 함");
                return null;
            }
        }

        return (grade, attr);
    }
    
    private AttributeType WeightedRandom(Dictionary<AttributeType, float> rates)
    {
        float total = 0f;
        foreach (var rate in rates.Values)
            total += rate;

        float roll = UnityEngine.Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var pair in rates)
        {
            cumulative += pair.Value;
            if (roll <= cumulative)
                return pair.Key;
        }

        return AttributeType.BaseAttack; // fallback
    }

}