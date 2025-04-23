using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : EnemyAction
{
    public SharedFloat moveRadius = 10f;
    public float rotationSpeed = 10;
    public float walkSpeedOffset = 3;
    private bool isWalking = false;
    
    private Vector3 destination;
    
    public override void OnStart()
    {
        SetRandomDestination();
        
        animator.PlayAnimationForState("walk", 0);
        agent.maxSpeed -= walkSpeedOffset;
        isWalking = true;
    }

    public override TaskStatus OnUpdate()
    {
        if (entityMovement.HasArrived)
        {
            return TaskStatus.Success; 
        }
        
        entityMovement.LookCheck();
        return TaskStatus.Running; 
    }

    public override void OnEnd()
    {
        if(isWalking)
            agent.maxSpeed += walkSpeedOffset;
    }

    private void SetRandomDestination()
    {
        destination = RandomNavMeshLocation();
        entityMovement.Destination = destination; 
    }

    private Vector3 RandomNavMeshLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveRadius.Value;
        randomDirection += transform.position;

        var nearestNodeInfo = AstarPath.active.GetNearest(randomDirection, NNConstraint.Default);
        return nearestNodeInfo.position;
    }
}
