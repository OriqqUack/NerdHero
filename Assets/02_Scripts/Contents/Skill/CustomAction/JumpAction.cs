using UnityEngine;
using Pathfinding;

[System.Serializable]
public class JumpAction : CustomAction
{
    private enum MethodType { Start, Run }

    [SerializeField] private MethodType method = MethodType.Run;
    [SerializeField] private float angle;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private bool isBackJump;
    [SerializeField] private AudioClip jumpSound;

    private Entity _entity;
    public override void Start(object data)
    {
        if (method == MethodType.Start)
        {
            var skillData = data as Skill;
            if (skillData == null) return;

            _entity = skillData.Owner;
            Jump();
        }
    }

    public override void Run(object data)
    {
        if (method == MethodType.Run)
        {
            var skillData = data as Skill;
            if (skillData == null) return;

            _entity = skillData.Owner;
            Jump();
        }
    }

    private void Jump()
    {
        var aiPath = _entity.GetComponent<FollowerEntity>();
        if (aiPath != null)
        {
            aiPath.canMove = false;
            aiPath.updatePosition = false;
        }

        _entity.Rigidbody.isKinematic = false;

        Vector3 direction = isBackJump ? -_entity.transform.forward : _entity.transform.forward;
        Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.Cross(direction, Vector3.up)) * direction;
        Vector3 forceDirection = rotatedDirection.normalized * jumpSpeed;

        _entity.Rigidbody.AddForce(forceDirection, ForceMode.Impulse);
        if(jumpSound)
            Managers.SoundManager.Play(jumpSound);
    }

    public override object Clone() => new JumpAction();
}