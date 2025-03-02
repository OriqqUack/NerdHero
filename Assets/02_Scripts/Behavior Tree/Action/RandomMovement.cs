using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : EnemyAction
{
    public SharedFloat moveRadius = 10f;
    public float rotationSpeed = 10;
    public float walkSpeedOffset = 3;
    
    private Vector3 destination;
    
    public override void OnStart()
    {
        SetRandomDestination();
        
        animator.PlayAnimationForState("walk", 0);
        agent.speed -= walkSpeedOffset;
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
        agent.speed += walkSpeedOffset;
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

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, moveRadius.Value, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return transform.position;
        }
    }
}
