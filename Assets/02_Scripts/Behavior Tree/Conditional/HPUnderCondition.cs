using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HPUnderCondition : EnemyCondition
{
    private bool isDead = false;
    public override void OnAwake()
    {
        base.OnAwake();
        entity.onDead += OnDead;
    }
    
    public override TaskStatus OnUpdate()
    {
        return isDead? TaskStatus.Success : TaskStatus.Failure;
    }

    private void OnDead(Entity entity)
    {
        isDead = true;
    }
}
