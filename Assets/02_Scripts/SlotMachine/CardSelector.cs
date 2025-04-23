using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardSelector
{
    private readonly CardBase[] _cardEffects = new CardBase[3];
    private Entity _entity;
    private CardDatabase _cardDatabase;
    private CardProbabilityManager _probabilityManager;
    // 속성 카테고리 지정 (예: 얼음, 불 등)
    private readonly AttributeType _attributeCategory = AttributeType.Element; // 예시: C가 속성 카테고리

    public CardSelector(Entity entity, CardDatabase cardDatabase, CardProbabilityManager probabilityManager)
    {
        _entity = entity;
        _cardDatabase = cardDatabase;
        _probabilityManager = probabilityManager;
    }

    public CardBase[] GetCardBases(int level, AttributeType recent, AttributeType previous, bool isFirst = false)
    {
        var commonRareHistory = new HashSet<AttributeType>();
        var selectedCards = new List<CardBase>();
        int attempts = 0;

        while (selectedCards.Count < 3 && attempts < 999)
        {
            var result = _probabilityManager.RollCard(level, commonRareHistory, _attributeCategory, recent, previous, isFirst);

            if (result.HasValue)
            {
                var card = _cardDatabase.GetRandomCard(result.Value.grade, result.Value.attr);
                if (card != null && !selectedCards.Contains(card))
                {
                    selectedCards.Add(card.Clone() as CardBase);

                    // 최근 등장 속성 업데이트
                    previous = recent;
                    recent = result.Value.attr;

                    if (result.Value.grade == EffectRarity.Common || result.Value.grade == EffectRarity.Rare)
                    {
                        commonRareHistory.Add(result.Value.attr);
                    }
                }
            }

            attempts++;
        }

        for (int i = 0; i < 3; i++)
        {
            _cardEffects[i] = (i < selectedCards.Count)
                ? selectedCards[i]
                : _cardDatabase.GetFallbackCard().Clone() as CardBase;
        }

        return _cardEffects;
    }
}