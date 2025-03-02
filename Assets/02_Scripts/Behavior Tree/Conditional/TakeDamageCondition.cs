using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class TakeDamageCondition : EnemyCondition
{
    public SharedString direction;
    public Stat isSuperArmor;

    private bool isTakeDamage = false;
    private bool hasSuperArmor = false;

    public override void OnAwake()
    {
        base.OnAwake();
        entity.onTakeDamage += TakeDamage;
        
    }

    public override void OnStart()
    {
        if (isSuperArmor == null) return;
        
        Debug.Log(entity.Stats.GetStat(isSuperArmor).Value);
        
        entity.Stats.GetStat(isSuperArmor).onValueChanged -= OnChangedStat;
        entity.Stats.GetStat(isSuperArmor).onValueChanged += OnChangedStat;
        
        if (Mathf.Approximately(entity.Stats.GetStat(isSuperArmor).Value, 1))
        {
            hasSuperArmor = true;
        }
    }
    
    public override TaskStatus OnUpdate()
    {
        if (hasSuperArmor)
            return TaskStatus.Failure;
        
        if (isTakeDamage)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }

    private void TakeDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        if(causer is string)
            direction.Value = (string)causer;
        
        entity.Target = instigator;
        
        isTakeDamage = true;
    }

    private void OnChangedStat(Stat stat, float value, float prevValue)
    {
        hasSuperArmor = Mathf.Approximately(value, 1) ? true : false;
        isTakeDamage = false;
    }

    public override void OnEnd()
    {
        isTakeDamage = false;
    }
}
