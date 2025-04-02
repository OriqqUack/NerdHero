using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSkillActionState : EntitySkillState
{
    public bool IsStateEnded { get; private set; }

    public override void Update()
    {
        // AnimatorParameter�� false��� State�� ����
        if (RunningSkill.InSkillActionFinishOption == InSkillActionFinishOption.FinishWhenAnimationEnded)
        {
            string anim = Entity.Animator.GetCurrentAnimation(1)?.Name;

            IsStateEnded = Entity.Animator.GetCurrentAnimation(1)?.Name != AnimatorParameterName;
        }
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // �ùٸ� Message�� �ƴ϶�� false�� return
        if (!base.OnReceiveMessage(message, data))
            return false;

        if (RunningSkill.InSkillActionFinishOption != InSkillActionFinishOption.FinishWhenAnimationEnded)
            RunningSkill.onApplied += OnSkillApplied;
        
        var tupleData = ((Skill, AnimatorParameter))data;
        Entity.Animator?.PlayOneShot(tupleData.Item2.name, 1);

        return true;
    }

    public override void Exit()
    {
        IsStateEnded = false;
        RunningSkill.onApplied -= OnSkillApplied;

        base.Exit();
    }

    private void OnSkillApplied(Skill skill, int currentApplyCount)
    {
        switch (skill.InSkillActionFinishOption)
        {
            // Skill�� �ѹ��̶� ����Ǿ��ٸ� State�� ����
            case InSkillActionFinishOption.FinishOnceApplied:
                IsStateEnded = true;
                break;

            // Skill�� ��� ����Ǿ��ٸ� State�� ����
            case InSkillActionFinishOption.FinishWhenFullyApplied:
                IsStateEnded = skill.IsFinished;
                break;
        }
    }
}
