using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntitySkillState : State<Entity>
{
    // ���� Entity�� �������� Skill
    public Skill RunningSkill { get; private set; }
    // Entity�� �����ؾ��� Animation�� Hash
    protected int AnimatorParameterHash { get; private set; }

    public override void Enter()
    {
        Entity.Movement?.Stop();

        var playerController = Entity.GetComponent<PlayerController>();
        if (playerController)
            playerController.enabled = false;
    }

    public override void Exit()
    {
        Entity.Animator?.SetBool(AnimatorParameterHash, false);

        RunningSkill = null;

        var playerController = Entity.GetComponent<PlayerController>();
        if (playerController)
            playerController.enabled = true;
    }
    
    public override bool OnReceiveMessage(int message, object data)
    {
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill, AnimatorParameter))data;
        
        RunningSkill = tupleData.Item1;
        AnimatorParameterHash = tupleData.Item2.Hash;
    
        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - �߸��� data�� ���޵Ǿ����ϴ�.");

        // Skill�� �ڽ��� ����� �������� ã�� ���¶��(=TargetSearcher.SelectTarget), �� ������ �ٶ�
        /*if (RunningSkill.IsTargetSelectSuccessful)
        {
            var selectionResult = RunningSkill.TargetSelectionResult;
            if (selectionResult.selectedTarget != Entity.gameObject)
                Entity.Movement.LookAtImmediate(selectionResult.selectedPosition);
        }*/

        Entity.Animator?.SetBool(AnimatorParameterHash, true);

        return true;
    }
}
