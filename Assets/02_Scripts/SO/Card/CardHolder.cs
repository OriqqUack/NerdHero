using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardfHolder_", menuName = "Card/CardHolder/CardHolder")]
public class CardHolder : ScriptableObject
{
    public List<CardBase> cards;
    
#if UNITY_EDITOR
    [ContextMenu("LoadCardBases")]
    private void LoadStats()
    {
        var stats = Resources.LoadAll<CardBase>("SOData/Card/CardBase").OrderBy(x => x.name);
        cards = stats.ToList();
    }
#endif
}
