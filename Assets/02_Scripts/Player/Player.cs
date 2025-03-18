using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        var skillSystem = GetComponent<SkillSystem>();
        if(GameManager.Instance.PlayerSkill1)
            skillSystem.RegisterWithoutCost(GameManager.Instance.PlayerSkill1);
        if(GameManager.Instance.PlayerSkill2)
            skillSystem.RegisterWithoutCost(GameManager.Instance.PlayerSkill2);
    }
}
