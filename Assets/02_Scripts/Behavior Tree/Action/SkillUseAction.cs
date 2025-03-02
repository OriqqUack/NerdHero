using System;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class SkillUseAction : EnemyAction
{
    public SharedTransform target;
    public int skillIndex = 0;
    private SkillSystem skillSystem;
    private Skill skill;
    private bool canUseSkill;
    public override void OnAwake()
    {
        base.OnAwake();
        skillSystem = GetComponent<SkillSystem>();
        skill = skillSystem.OwnSkills[skillIndex];
    }

    public override void OnStart()
    {
        entityMovement.Stop();
        if (skill.IsInState<ReadyState>())
        {
            canUseSkill = skill.Use();
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (canUseSkill)
            return TaskStatus.Success;
        else
            return TaskStatus.Failure;
    }
}
