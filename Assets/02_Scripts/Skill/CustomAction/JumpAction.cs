using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class JumpAction : CustomAction
{
    [SerializeField] private float angle;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private bool isBackJump;

    private Entity _entity;
    public override void Run(object data)
    {
        var skillData = data as Skill;
        if (skillData == null) return;

        _entity = skillData.Owner;
        Jump();
    }
    
    private void Jump()
    {
        _entity.GetComponent<NavMeshAgent>().updatePosition = false;
        
        Vector3 direction = isBackJump ? -_entity.transform.forward : _entity.transform.forward;
        Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.Cross(direction, Vector3.up)) * direction;
        Vector3 forceDirection = rotatedDirection.normalized * jumpSpeed;
        _entity.Rigidbody.AddForce(forceDirection, ForceMode.Impulse);
    }

    public override object Clone() => new JumpAction();
}
