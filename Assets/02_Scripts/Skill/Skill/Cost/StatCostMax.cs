using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatCostMax : Cost
{
    [SerializeField]
    private Stat stat;

    public override string Description => stat.DisplayName;

    public override bool HasEnoughCost(Entity entity)
        => entity.Stats.GetValue(stat) >= entity.Stats.GetStat(stat).MaxValue;

    public override void UseCost(Entity entity)
        => entity.Stats.IncreaseDefaultValue(stat, -entity.Stats.GetStat(stat).Value);

    public override void UseDeltaCost(Entity entity)
        => entity.Stats.IncreaseDefaultValue(stat, -entity.Stats.GetStat(stat).Value * Time.deltaTime);

    public override float GetValue(Entity entity) => entity.Stats.GetStat(stat).Value;

    public override object Clone()
        => new StatCostMax() { stat = stat };
}