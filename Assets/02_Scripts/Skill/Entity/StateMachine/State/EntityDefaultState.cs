using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDefaultState : State<Entity>
{
    public override bool OnReceiveMessage(int message, object data)
    {
        if ((EntityStateMessage)message != EntityStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill skill, AnimatorParameter animatorParameter))data;
        Entity.Animator?.PlayAnimationForState(tupleData.Item2.name, 0);

        return true;
    }
}
