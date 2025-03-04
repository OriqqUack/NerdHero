using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class TracingTimerAction : EnemyAction
{
    public float TracingTime;
    [SerializeField] private Stat attackRangeStat;

    private float elapsedTime;
    
    public override void OnStart()
    {
        entityMovement.TraceTarget = targetTransform;
        if(TracingTime == 0)
            TracingTime = Mathf.Infinity;
        
        attackRangeStat = entity.Stats.GetStat(attackRangeStat);
        animator.PlayAnimationForState("run", 0);
    }
    public override TaskStatus OnUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= TracingTime)
        {
            return TaskStatus.Success;
        }
        
        if (entityMovement.HasArrived)
        {
            entityMovement.TraceTarget = targetTransform;
        }
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        elapsedTime = 0;
    }
    
}
