using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSkillActionState : EntitySkillState
{
    public bool IsStateEnded { get; private set; }
    private int layerIndex;

    public override void Update()
    {
        // AnimatorParameter�� false��� State�� ����
        if (RunningSkill.InSkillActionFinishOption == InSkillActionFinishOption.FinishWhenAnimationEnded)
        {
            /*string anim = Entity.Animator.GetCurrentAnimation(1)?.Name;
            Debug.Log($"Current Animation Name : " + anim);*/
            IsStateEnded = Entity.Animator.GetCurrentAnimation(layerIndex)?.Name != AnimatorParameterName;
        }
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // �ùٸ� Message�� �ƴ϶�� false�� return
        if (!base.OnReceiveMessage(message, data))
            return false;

        if (RunningSkill.InSkillActionFinishOption != InSkillActionFinishOption.FinishWhenAnimationEnded)
        {
            RunningSkill.onApplied += OnSkillApplied;
        }
        
        if (RunningSkill.InSkillActionFinishOption == InSkillActionFinishOption.FinishWhenAnimationEnded)
        {
            Entity.Movement.enabled = false;
        }
        
        var tupleData = ((Skill, AnimatorParameter))data;
        layerIndex = (int)tupleData.Item2.index;
        float statValue = 1f;
        if(tupleData.Item2.stat)
        {
            var stat = Entity.Stats.GetStat(tupleData.Item2.stat);
            statValue = statValue + statValue * stat.Value;
        }
        Entity.Animator?.PlayOneShot(tupleData.Item2.name, layerIndex, 0, null, statValue);
        return true;
    }

    public override void Exit()
    {
        IsStateEnded = false;
        Entity.Movement.enabled = true;
        Entity.CanTakeDamage = true;

        layerIndex = 0;
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
