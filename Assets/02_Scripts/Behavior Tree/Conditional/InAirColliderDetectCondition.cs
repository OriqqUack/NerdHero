using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[RequireComponent(typeof(GroundCheck))]
public class InAirColliderDetectCondition : EnemyCondition
{
    public GameObject vfxPrefab;
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

        if (isColliding)
        {
            var go = Managers.Resource.Instantiate(vfxPrefab);
            go.transform.position = transform.position;
            return TaskStatus.Success;
        }
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        isColliding = false;
    }
}
