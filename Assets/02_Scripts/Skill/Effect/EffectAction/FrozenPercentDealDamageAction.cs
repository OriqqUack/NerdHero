using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrozenPercentIncreaseStatAction : EffectAction
{
    [SerializeField]
    private Stat stat;
    [SerializeField] 
    private bool isPercent;
    [SerializeField]
    private float defaultValue;
    
    private float GetBonusStatValue(Entity user)
        => user.Stats.GetValue(stat) * defaultValue;
    
    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        var list = target.SkillSystem.Find("SLOW");
        if (list.Count == 0) return true;
        float total = GetBonusStatValue(user);
        target.Stats.SetBonusValue(stat, this, total);
        return true;
    }

    public override object Clone()
    {
        return new FrozenPercentIncreaseStatAction()
        {
            stat = stat,
            isPercent = isPercent,
            defaultValue = defaultValue
        };
    }
}