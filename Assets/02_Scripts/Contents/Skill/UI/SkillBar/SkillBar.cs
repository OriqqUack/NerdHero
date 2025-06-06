using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBar : MonoSingleton<SkillBar>
{
    [SerializeField]
    private GameObject slotPrefab;
    [SerializeField]
    private int slotCount;

    private SkillSystem skillSystem;
    public List<SkillSlot> slots = new();
    private int emptySlotIndex;

    private void Start()
    {
        skillSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<SkillSystem>();
        skillSystem.onSkillRegistered += OnSkillRegistered;

        var ownSkills = skillSystem.OwnSkills;

        //기본 공격 제외
        for (int i = 1; i <= slotCount; i++)
        {
            var slot = Instantiate(slotPrefab, transform).GetComponent<SkillSlot>();
            slot.Setup();
            slots.Add(slot);

            if (i < ownSkills.Count)
                TryAddToEmptySlot(ownSkills[i]);
        }
    }

    private void OnDestroy() => skillSystem.onSkillRegistered -= OnSkillRegistered;


    private void TryAddToEmptySlot(Skill skill)
    {
        if (emptySlotIndex >= slotCount || skill.IsPassive)
            return;
        
        slots[emptySlotIndex++].Skill = skill;
    }

    private void OnSkillRegistered(SkillSystem skillSystem, Skill skill)
        => TryAddToEmptySlot(skill);
}
