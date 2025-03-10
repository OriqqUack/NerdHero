using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class StraightMoveAction : EnemyAction
{
    public float moveDistance = 10f;
    public float runSpeedOffset = 3f;

    private Collider collider;
    public override void OnStart()
    {
        entityMovement.ForceStop();
        entityMovement.Destination = transform.position + transform.forward * moveDistance;

        collider.isTrigger = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
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
        collider.isTrigger = false;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        agent.speed -= runSpeedOffset;
    }
}
