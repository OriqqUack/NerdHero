using System.Collections.Generic;
using UnityEngine;

public class CardDatabase
{
    private List<CardBase> allCards;

    public CardDatabase(CardHolder cardHolder)
    {
        this.allCards = new List<CardBase>(cardHolder.cards);
    }
    
    public CardBase GetRandomCard(EffectRarity grade, AttributeType attr)
    {
        var filtered = allCards.FindAll(card => card.EffectSO.Rarity == grade && card.attributeType == attr);
        if (filtered.Count == 0) return null;
        return filtered[UnityEngine.Random.Range(0, filtered.Count)];
    }

    public CardBase GetFallbackCard()
    {
        return allCards[0]; // or a specific "dummy" card
    }
}