using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FirePercentDealDamageAction : EffectAction
{
    [SerializeField]
    private float defaultDamage;
    [SerializeField] 
    private float percent;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        if (Random.value >= percent) return true;
        var list = target.SkillSystem.Find("FIRE");
        if (list.Count == 0) return true;
        target.TakeDamage(user, effect, defaultDamage);

        return true;
    }

    public override object Clone()
    {
        return new FirePercentDealDamageAction()
        {
            defaultDamage = defaultDamage,
            percent = percent
        };
    }
}
