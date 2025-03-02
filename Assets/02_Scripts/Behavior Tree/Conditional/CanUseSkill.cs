using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class CanUseSkill : EnemyCondition
{
    public int skillIndex = 0;
    private SkillSystem skillSystem;
    private Skill skill;
    
    public override void OnAwake()
    {
        base.OnAwake();
        skillSystem = GetComponent<SkillSystem>();
        skill = skillSystem.OwnSkills[skillIndex];
    }

    public override TaskStatus OnUpdate()
    {
        if(skill.IsUseable && skill.IsCooldownCompleted)
            return TaskStatus.Success;
        
        return TaskStatus.Failure;
    }
}
