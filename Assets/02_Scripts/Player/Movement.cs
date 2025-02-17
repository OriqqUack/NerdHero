using Spine.Unity.Examples;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Moving")]
    [SerializeField] protected Stat moveSpeed;
    [SerializeField] protected float gravityScale = 9.81f;
    [SerializeField] protected float walkSpeedOffset = 3f;
    
    
    [Space(10)]
    [Header("Animation")]
    [SerializeField] protected SkeletonAnimationHandleExample animationHandle;
    
    [Space(10)]
    [Header("Components")]
    public CharacterController Controller;

    protected float walkSpeed => Mathf.Clamp(moveSpeed.MaxValue - walkSpeedOffset, 1f, moveSpeed.MaxValue);
    protected float runSpeed => moveSpeed.Value;
    protected Entity entity;
    protected Transform traceTarget;
    
    public virtual void Setup(Entity entity)
    {
        this.entity = entity;
    }

    public virtual void Stop()
    {
        Controller.Move(Vector3.zero);
    }
}
