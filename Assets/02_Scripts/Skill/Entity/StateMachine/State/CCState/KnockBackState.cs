using UnityEngine;

public class KnockBackState : EntityCCState
{
    private static readonly int kAnimationHash = Animator.StringToHash("isKnockBack");
    public override string Description => "밀쳐짐";
    protected override int AnimationHash => kAnimationHash;

    private float _knockBackValue => Entity.Stats.GetStat("KnockBack").Value;
    public override void Enter()
    {
        base.Enter();
        //Entity.Movement.Controller.Move()
    }
}
