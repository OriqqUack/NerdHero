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
    protected string AnimatorParameterName { get; private set; }
    
    public override void Enter()
    {
        Entity.Movement?.Stop();

        var playerController = Entity.GetComponent<PlayerController>();
        if (playerController)
            playerController.enabled = false;
    }

    public override void Exit()
    {
        RunningSkill = null;
        Entity.Movement?.ReStart();
        
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
        AnimatorParameterName = tupleData.Item2.name;
        
        Debug.Assert(RunningSkill != null,
            $"CastingSkillState({message})::OnReceiveMessage - �߸��� data�� ���޵Ǿ����ϴ�.");

        // Skill�� �ڽ��� ����� �������� ã�� ���¶��(=TargetSearcher.SelectTarget), �� ������ �ٶ�
        /*if (RunningSkill.IsTargetSelectSuccessful)
        {
            var selectionResult = RunningSkill.TargetSelectionResult;
            if (selectionResult.selectedTarget != Entity.gameObject)
                Entity.Movement.LookAtImmediate(selectionResult.selectedPosition);
        }*/

        Entity.Animator?.PlayOneShot(tupleData.Item2.name, 1);

        return true;
    }
}
