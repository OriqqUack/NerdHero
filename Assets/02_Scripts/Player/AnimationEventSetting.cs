using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class AnimationEventSetting : MonoBehaviour
{
    [SerializeField] private List<string> skillEventName;
    [SerializeField] private List<string> nonSkillEventName;

    [SerializeField] private GameObject runningEffect;

    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string onEndName;

    
    private Entity _entity;
    private SkillSystem _skillSystem;
    private Spine.AnimationState _animationState;
    private SkeletonAnimation skeletonAnimation;
    private Transform _footStepTs;
    
    void Start()
    {
        _entity = GetComponent<Entity>();
        _skillSystem = _entity.SkillSystem;

        skeletonAnimation = _entity.Animator.skeletonAnimation;
        _animationState = skeletonAnimation.AnimationState;
        _animationState.Event += HandleAnimationStateEventSkill;
        _animationState.Event += HandleAnimationStateEventNonSkill;
        
        _footStepTs = transform.Find("FootStep");
    }
    
    private void HandleAnimationStateEventSkill(TrackEntry trackentry, Spine.Event e)
    {
        foreach (var eventName in skillEventName)
        {
            if (e.Data.Name == eventName)
            {
                _skillSystem.ApplyCurrentRunningSkill();
            }
        }
    }
    
    private void HandleAnimationStateEventNonSkill(TrackEntry trackentry, Spine.Event e)
    {
        foreach (var eventName in nonSkillEventName)
        {
            if (e.Data.Name == eventName)
            {
                OnPlay(eventName);
            }
        }
    }

    private void OnPlay(string eventName)
    {
        switch (eventName)
        {
            case "move":
                SyncMove();
                break;
            case "footstep":
                CreateRunEffect();
                break;
        }
    }

    private void SyncMove()
    {
        Transform ts = _entity.GetTransformSocket("pelvis");
        _entity.transform.position = ts.position;
    }

    private void CreateRunEffect()
    {
        GameObject effect = Managers.Resource.Instantiate(runningEffect, _footStepTs.position, Quaternion.Euler(0, 0, 0));
    }
    
}
