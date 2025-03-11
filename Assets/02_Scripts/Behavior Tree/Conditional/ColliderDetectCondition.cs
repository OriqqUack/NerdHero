using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class ColliderDetectCondition : EnemyCondition
{
    private GroundCheck _groundCheck;
    private bool isColliding = false;

    public override void OnAwake()
    {
        _groundCheck = GetComponent<GroundCheck>();
    }
    
    public override TaskStatus OnUpdate()
    {
        return isColliding ? TaskStatus.Success : TaskStatus.Running;
    }
    
    public override void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            isColliding = true;
    }
    
    public override void OnEnd()
    {
        isColliding = false;
    }
}
