using UnityEngine;

[System.Serializable]
public class UnderHpCondition : CardCondition
{
    public float hpRate;
    
    public override bool IsPass(Skill skill)
    {
        var entity = skill.Owner;
        var stat = entity.Stats.GetStat("HP");
        return stat.MaxValue * hpRate >= stat.Value;
    }

    public override object Clone()
    => new UnderHpCondition();
}
