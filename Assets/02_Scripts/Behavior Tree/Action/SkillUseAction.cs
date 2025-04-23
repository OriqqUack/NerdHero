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
    public string animationName;
    public int skillIndex = 0;
    public bool IsMoveSkill = false;
    
    private SkillSystem skillSystem;
    private Skill skill;
    private bool canUseSkill;
    private bool isUse;
    
    public override void OnAwake()
    {
        base.OnAwake();
        skillSystem = GetComponent<SkillSystem>();
        Debug.Assert(skillSystem.OwnSkills[skillIndex] != null, $"Skill Index : {skillIndex} Not Exist");
        skill = skillSystem.OwnSkills[skillIndex];
    }

    public override void OnStart()
    {
        entityMovement.StopTracing();
        Play();
        if(!string.IsNullOrEmpty(animationName))
            entity.Animator.PlayOneShot(animationName, 0);
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
        if(IsMoveSkill && isUse)
            return TaskStatus.Success;
        
        if (skill.IsInState<CooldownState>())
            return TaskStatus.Success;
        
        if(isUse && !canUseSkill)
            return TaskStatus.Failure;
        
        return TaskStatus.Running;
    }

    public override void OnEnd()
    {
        canUseSkill = false;
        isUse = false;
    }
}
