using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class BaseAttackCheck : MonoBehaviour
{
    private Entity _entity;
    private SkillSystem _skillSystem;
    private Skill _baseSkill;
    
    public void Setup(Entity entiy)
    {
        _entity = entiy;
        _skillSystem = _entity.SkillSystem;
        _baseSkill = _skillSystem.OwnSkills[0];
    }
    
    private void OnTriggerEnter(Collider other)
    {
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
        {
            other.GetComponent<EntityHUD>().AxisImageControl(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_baseSkill.IsInState<ReadyState>())
        {
            _baseSkill.Use();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
        {
            other.GetComponent<EntityHUD>().AxisImageControl(false);
        }
    }
}
