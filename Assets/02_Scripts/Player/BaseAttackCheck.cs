using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class BaseAttackCheck : MonoBehaviour
{
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string onEndName;
    
    private Entity _entity;
    private SkillSystem _skillSystem;
    private Skill _baseSkill;
    private Spine.AnimationState _animationState;
    private SkeletonAnimation skeletonAnimation;
    
    public void Setup(Entity entiy)
    {
        _entity = entiy;
        _skillSystem = _entity.SkillSystem;
        _baseSkill = _skillSystem.OwnSkills[0];

        skeletonAnimation = _entity.Animator.skeletonAnimation;
        _animationState = skeletonAnimation.AnimationState;
        _animationState.Event += HandleAnimationStateEvent;
    }

    private void HandleAnimationStateEvent(TrackEntry trackentry, Spine.Event e)
    {
        if (e.Data.Name == onEndName)
        {
            _skillSystem.ApplyCurrentRunningSkill();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
        {
            other.GetComponent<EntityHUD>().AxisImageControl(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_baseSkill.IsInState<ReadyState>())
        {
            _baseSkill.Use();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
        {
            other.GetComponent<EntityHUD>().AxisImageControl(false);
        }
    }
}
