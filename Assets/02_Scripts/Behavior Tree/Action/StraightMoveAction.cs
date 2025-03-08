using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class StraightMoveAction : EnemyAction
{
    public float moveDistance = 10f;
    public float runSpeedOffset = 3f;

    public override void OnStart()
    {
        entityMovement.ForceStop();
        entityMovement.Destination = transform.position + transform.forward * moveDistance;
        
        agent.speed += runSpeedOffset;
    }

    public override TaskStatus OnUpdate()
    {
        if (entityMovement.HasArrived)
        {
            return TaskStatus.Success; 
        }
        
        return TaskStatus.Running; 
    }

    public override void OnEnd()
    {
        agent.speed -= runSpeedOffset;
    }
}
