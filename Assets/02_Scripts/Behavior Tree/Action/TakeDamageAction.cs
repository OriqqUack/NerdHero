using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class TakeDamageAction : EnemyAction
{
    private bool isEnd;
    public override void OnStart()
    {
        animator.PlayOneShot("damaged", 0, 0, () => AnimationEnd());
        entityMovement.StopMoment();
    }

    public override TaskStatus OnUpdate()
    {
        if(isEnd)
            return TaskStatus.Success;
        
        return TaskStatus.Running;
    }
    
    public override void OnEnd()
    {
        entityMovement.RestartMovement();
    }
    
    private void AnimationEnd()
    {
        isEnd = true;
    }
}
