using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : State<Skill>
{
    // Skill�� ������ Owner�� StateMachine���� ���� ��ȯ Command�� SKill�� ������ ������ �Լ�
    protected void TrySendCommandToOwner(Skill skill, EntityStateCommand command, AnimatorParameter animatorParameter)
    {
        var ownerStateMachine = Entity.Owner.StateMachine;
        if (ownerStateMachine != null && animatorParameter.IsValid)
        {
            // ���ڷ� ���� animatorParameter�� bool Type�̸� owner�� StateMachine���� ���ڷ� ���� command�� ����
            // Transition�� Command�� �޾Ƶ鿴����, State�� UsingSKill Message�� Skill ������ ����
            if (animatorParameter.type == AnimatorParameterType.Bool && ownerStateMachine.ExecuteCommand(command))
                ownerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            // ���ڷ� ���� animatorParameter�� trigger Type�̸� �ൿ�� ������ ���� ���� ���̹Ƿ� ToDefaultState Command�� ������
            // Transition�� �޾Ƶ鿴������ �������, State�� UsingSkill Message�� skill ������ ����
            else if (animatorParameter.type == AnimatorParameterType.Trigger)
            {
                ownerStateMachine.ExecuteCommand(EntityStateCommand.ToDefaultState);
                ownerStateMachine.SendMessage(EntityStateMessage.UsingSkill, (skill, animatorParameter));
            }
        }
    }
}