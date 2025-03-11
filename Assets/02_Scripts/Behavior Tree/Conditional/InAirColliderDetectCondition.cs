using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[RequireComponent(typeof(GroundCheck))]
public class InAirColliderDetectCondition : EnemyCondition
{
    private bool isColliding = false;
    private GroundCheck _groundCheck;

    public override void OnAwake()
    {
        _groundCheck = GetComponent<GroundCheck>();
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            isColliding = true;
    }

    public override TaskStatus OnUpdate()
    {
        if(_groundCheck.IsOnGround)
            return TaskStatus.Failure;
        
        return isColliding ? TaskStatus.Success : TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isColliding = false;
    }
}
