using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingSkillState : EntitySkillState
{
    public override bool OnReceiveMessage(int message, object data)
    {
        if (!base.OnReceiveMessage(message, data))
            return false;
        
        var tupleData = ((Skill, AnimatorParameter))data;
        RunningSkill = tupleData.Item1;
        Entity.Animator?.PlayOneShot(tupleData.Item2.name, 1);
        return true;
    }
}
