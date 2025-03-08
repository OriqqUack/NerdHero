using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.AI;
using Event = UnityEngine.Event;

public class Slime : MonoBehaviour
{
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string onStartName;
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string onEndName;

    private Entity entity;
    private NavMeshAgent navMeshAgent;
    private EntityMovement entityMovement;
    Spine.EventData eventData;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        entityMovement = GetComponent<EntityMovement>();
    }
    private void Start()
    {
        entity.SkeletonAnimator.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
    }

    private void HandleAnimationStateEvent(TrackEntry trackentry, Spine.Event e)
    {
        if (trackentry.Animation.Name != "move") return;
        if(e.Data.Name == onStartName)
            navMeshAgent.isStopped = false;
        else if(e.Data.Name == onEndName)
            navMeshAgent.isStopped = true;
    }

}
