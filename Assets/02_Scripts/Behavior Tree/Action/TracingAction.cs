using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class TracingAction : EnemyAction
{
    public float TracingTime;

    private float elapsedTime;
    
    public override void OnStart()
    {
        entityMovement.TraceTarget = targetTransform;
        if(TracingTime == 0)
            TracingTime = Mathf.Infinity;
        
        animator.PlayAnimationForState("run", 0);
    }
    
    public override TaskStatus OnUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= TracingTime)
        {
            return TaskStatus.Failure;
        }
        
        if (entityMovement.HasArrived)
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