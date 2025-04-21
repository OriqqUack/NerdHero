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
    private List<CardProbabilityConfig> gradeProbConfigs;

    public CardProbabilityManager(List<CardProbabilityConfig> gradeProbConfigs)
    {
        this.gradeProbConfigs = gradeProbConfigs;
    }
    
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
        HashSet<AttributeType> commonRareHistory,
        AttributeType attributeCategory)
    {
        var config = gradeProbConfigs.Find(c => c.level == level);
        if (config == null) return null;

        // 1. 등급 결정
        float r = UnityEngine.Random.value * 100f;
        EffectRarity grade;
        if (r < config.commonRate) grade = EffectRarity.Common;
        else if (r < config.commonRate + config.rareRate) grade = EffectRarity.Rare;
        else grade = EffectRarity.Legendary;

        // 2. 속성 결정 (동등 확률)
        Array values = Enum.GetValues(typeof(AttributeType));
        AttributeType attr = (AttributeType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

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
}