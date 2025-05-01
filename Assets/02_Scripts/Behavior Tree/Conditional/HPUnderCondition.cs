using BehaviorDesigner.Runtime.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class HpUnderCondition : EnemyCondition
{
    [SerializeField] private float hpRate;
    private bool _conditionCheck;
    public override void OnAwake()
    {
        base.OnAwake();
        entity.Stats.HPStat.onValueChanged += StatChanged;
    }

    public override TaskStatus OnUpdate()
    {
        if(_conditionCheck)
            return TaskStatus.Success;

        return TaskStatus.Failure;
    }

    public override void OnEnd()
    {
        _conditionCheck = false;
    }

    private void StatChanged(Stat stat, float currentValue, float prevValue)
    {
        if (hpRate >= (stat.Value / stat.MaxValue))
            _conditionCheck = true;
    }
}
