using BehaviorDesigner.Runtime.Tasks;
using Spine.Unity.Examples;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAction : Action
{
    protected Entity entity;
    protected EntityMovement entityMovement;
    protected SkeletonAnimationHandleExample animator;
    protected Rigidbody rigidbody;
    protected Collider collider;
    protected NavMeshAgent agent;
    protected Transform targetTransform;
    
    public override void OnAwake()
    {
        entity = GetComponent<Entity>();
        entityMovement = GetComponent<Movement>() as EntityMovement;
        animator = GetComponent<SkeletonAnimationHandleExample>();
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        targetTransform = WaveManager.Instance.PlayerTransform;
    }
}
