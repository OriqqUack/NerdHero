using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class TracingAction : EnemyAction
{
    public SharedTransform target;
    public float TracingTime;
    [SerializeField] private Stat attackRangeStat;

    private float elapsedTime;
    
    public override void OnStart()
    {
        entityMovement.TraceTarget = target.Value;
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
            return TaskStatus.Failure;
        }

        if (entityMovement.HasArrived || Vector3.Distance(target.Value.position, transform.position) <= attackRangeStat.Value)
        {
            entityMovement.ForceStop();
            return TaskStatus.Success;
        }
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        elapsedTime = 0;
    }
}