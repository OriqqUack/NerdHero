using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class OnAirCondition : EnemyCondition
{
    private GroundCheck _groundCheck;
    
    public override void OnAwake()
    {
        _groundCheck = GetComponent<GroundCheck>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!_groundCheck.IsOnGround)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}
