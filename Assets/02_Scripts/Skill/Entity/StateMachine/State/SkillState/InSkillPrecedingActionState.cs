using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSkillPrecedingActionState : EntitySkillState
{
    public override void Update()
    {
        // AnimatorParameter�� false��� State�� ����
    }
    
    public override bool OnReceiveMessage(int message, object data)
    {
        if (!base.OnReceiveMessage(message, data))
            return false;
        var tupleData = ((Skill, AnimatorParameter))data;
        Entity.Animator?.PlayOneShot(tupleData.Item2.name, 1);
        return true;
    }
}
