using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardSelector
{
    private List<CardBase> _cardList;
    private readonly CardBase[] _cardEffects = new CardBase[3];
    private Entity _entity;
    public CardSelector(Entity entity, CardHolder cardHolder)
    {
        Debug.Assert(cardHolder.cards != null, "Card Holder가 Null이 될 순 없습니다.");
        _cardList = new List<CardBase>(cardHolder.cards);
        _entity = entity;
    }

    public CardBase[] GetCardBases()
    {
        // 중복 없는 인덱스 3개 뽑기
        List<int> indices = new List<int>();
        while (indices.Count < 3)
        {
            int rand = Random.Range(0, _cardList.Count);
            if (!indices.Contains(rand))
            {
                indices.Add(rand);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            _cardEffects[i] = _cardList[indices[i]].Clone() as CardBase;
        }
        
        return _cardEffects;
    }
}