using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
[System.Serializable]
public class DestroySelfAction : CustomAction
{
    public override void Run(object data)
    {
        Skill skill = (Skill)data;
        if (!skill) return;
        Stat stat = skill.Owner.Stats.GetStat("HP");
        float maxHp = stat.MaxValue;
        skill.Owner.TakeDamage(skill.Owner, null, maxHp);
    }
    
    public override object Clone() => new DestroySelfAction();
}
