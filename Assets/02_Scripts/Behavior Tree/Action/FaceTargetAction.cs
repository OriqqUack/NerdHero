using UnityEngine;

public class FaceTargetAction : EnemyAction
{
    public override void OnStart()
    {
        entityMovement.LookCheck();
    }
}
