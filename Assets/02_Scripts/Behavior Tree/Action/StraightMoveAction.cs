using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class StraightMoveAction : EnemyAction
{
    public float moveDistance = 10f;
    public float runSpeedOffset = 3f;
    private bool isColliding = false;

    private Collider collider;
    public override void OnStart()
    {
        entityMovement.ForceStop();
        entityMovement.Destination = transform.position + transform.forward * moveDistance;

        agent.speed += runSpeedOffset;
        animator.PlayAnimationForState("sliding", 0);
    }
    
    public override void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            isColliding = true;
    }

    public override TaskStatus OnUpdate()
    {
        if (entityMovement.HasArrived)
        {
            return TaskStatus.Success; 
        }

        if (isColliding)
        {
            entityMovement.ForceStop();
            return TaskStatus.Success; 
        }
        
        return TaskStatus.Running; 
    }

    public override void OnEnd()
    {
        agent.speed -= runSpeedOffset;
        isColliding = false;
    }
}
