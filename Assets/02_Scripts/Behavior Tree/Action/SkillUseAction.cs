using System;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;

public class SkillUseAction : EnemyAction
{
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string eventName;

    public string animationName;
    public int skillIndex = 0;
    
    private SkillSystem skillSystem;
    private Skill skill;
    private bool canUseSkill;
    private bool isUse;
    Spine.EventData eventData;
    
    public bool logDebugMessage = false;
    public override void OnAwake()
    {
        base.OnAwake();
        skillSystem = GetComponent<SkillSystem>();
        skill = skillSystem.OwnSkills[skillIndex];
    }

    public override void OnStart()
    {
        entityMovement.ForceStop();
        if (eventName == null)
        {
            Play();
            if(animationName != null)
                entity.Animator.PlayOneShot(animationName, 0);
        }
        else
        {
            eventData = entity.Animator.skeletonAnimation.Skeleton.Data.FindEvent(eventName);
            entity.Animator.skeletonAnimation.AnimationState.Event -= HandleAnimationStateEvent;
            entity.Animator.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
            entity.Animator.PlayOneShot(animationName, 0);
        }
    }
    
    private void HandleAnimationStateEvent (TrackEntry trackEntry, Event e) 
    {
        if (logDebugMessage) Debug.Log("Event fired! " + e.Data.Name);
        //bool eventMatch = string.Equals(e.Data.Name, eventName, System.StringComparison.Ordinal); // Testing recommendation: String compare.
        bool eventMatch = (eventData == e.Data); // Performance recommendation: Match cached reference instead of string.
        string animationOriginalName = entity.Animator.GetAnimationForState(animationName).Name;
        bool animationMatch = trackEntry.Animation.Name == animationOriginalName;
        if (eventMatch && animationMatch) 
        {
            Play();
        }
    }
    
    public void Play () 
    {
        if (skill.IsInState<ReadyState>())
        {
            canUseSkill = skill.Use();
            isUse = true;
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (canUseSkill)
            return TaskStatus.Success;
        
        if(isUse && !canUseSkill)
            return TaskStatus.Failure;
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        entity.Animator.skeletonAnimation.AnimationState.Event -= HandleAnimationStateEvent;
        canUseSkill = false;
        isUse = false;
    }
}
