using UnityEngine;

public class KnockBackState : EntityCCState
{
    private static readonly int kAnimationHash = Animator.StringToHash("isKnockBack");
    public override string Description => "밀쳐짐";
    protected override int AnimationHash => kAnimationHash;
}
