using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryManager : MonoSingleton<InventoryManager>, ISaveable
{
    private List<ItemSO> items = new List<ItemSO>();
    private List<Skill> skillList = new List<Skill>();
    private List<Skill> _filteredSkillList = new List<Skill>();

    public List<ItemSO> GetItems() => items;

    public void SetupSkills()
    {
        _filteredSkillList.Clear();
        foreach (Skill skill in skillList)
        {
            var cloneSkill = skill.Clone() as Skill;
            cloneSkill.Setup(GameManager.Instance.GetPlayerEntity());
            _filteredSkillList.Add(cloneSkill);
        }
    }
    
    public List<Skill> GetSkills()
    {
        SetupSkills();
        return _filteredSkillList;
    }
    
    public void AddItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            int index = items.IndexOf(item);
            if (items[index].itemType == ItemType.Stuff)
                items[index].quantityOrLevel += item.quantityOrLevel;
            else
                items.Add(item);
        }
        else
        {
            items.Add(item);
        }
    }

    public void AddSkill(Skill skill)
    {
        if (skillList.Contains(skill)) return;
        skillList.Add(skill);
    }
    
    public void RemoveItem(ItemSO item)
    {
        if (items.Contains(item))
        {
            int index = items.IndexOf(item);
            if (items[index].itemType == ItemType.Stuff)
            {
                items[index].quantityOrLevel -= item.quantityOrLevel;
                if (items[index].quantityOrLevel <= 0)
                    items.RemoveAt(index);
            }
            else
                items.RemoveAt(index);
        }
    }

    [ContextMenu("Clear Inventory")]
    public void ClearInventory()
    {
        items.Clear(); // 아이템 목록 비우기
        skillList.Clear(); // 스킬 목록 비우기
        _filteredSkillList.Clear(); // 필터링된 스킬 목록 비우기
    }

    public void Save(SaveData data)
    {
        Equipment equipment = Equipment.Instance;

        data.inventoryItems.Clear();
        data.quantityOrLevel.Clear();
        foreach (ItemSO item in items)
        {
            data.inventoryItems.Add(item.itemName);
            data.quantityOrLevel.Add(item.quantityOrLevel);
        }
        
        if(equipment)
        {
            data.equippedWeapon = equipment.weapon?.itemName;
            if (equipment.weapon != null) data.weaponLevel = equipment.weapon.quantityOrLevel;

            data.equippedArmor = equipment.armor?.itemName;
            if (equipment.armor != null) data.armorLevel = equipment.armor.quantityOrLevel;

            data.equippedHelmet = equipment.helmet?.itemName;
            if (equipment.helmet != null) data.helmetLevel = equipment.helmet.quantityOrLevel;

            data.equippedBoots = equipment.boots?.itemName;
            if (equipment.boots != null) data.bootsLevel = equipment.boots.quantityOrLevel;
        }
        
        data.skills.Clear();
        data.skillLevels.Clear();
        foreach (Skill skill in _filteredSkillList)
        {
            data.skills.Add(skill.CodeName);
            data.skillLevels.Add(skill.Level);
        }
        
        if(equipment?.skill1)
            data.equippedSkill1 = equipment.skill1.CodeName;
        if(equipment?.skill2)
            data.equippedSkill2 = equipment.skill2.CodeName;
    }

    public void Load(SaveData data)
    {
        if (data == null) return;
        Equipment equipment = Equipment.Instance;
        //인벤토리 로드
        items.Clear();
        int index = 0;
        foreach (string itemName in data.inventoryItems)
        {
            ItemSO item = ItemDatabase.Instance.GetItemByName(itemName);
            if (item != null)
            {
                item.quantityOrLevel = data.quantityOrLevel[index];
                items.Add(item);
            }
            index++;
        }

        //Skill
        skillList.Clear();
        for (int i = 0; i < data.skills.Count; i++)
        {
            skillList.Add(Resources.Load<Skill>($"Skill/SKILL_{data.skills[i]}"));
        }
        SetupSkills();
        for (int i = 0; i < _filteredSkillList.Count; i++)
        {
            _filteredSkillList[i].Level = data.skillLevels[i];
        }
            
        //EquippedSkill 불러오기
        if(!string.IsNullOrEmpty(data.equippedSkill1))
        {
            var skill = _filteredSkillList.Find((x) => x.CodeName == data.equippedSkill1);
            equipment.Equip(skill, 0);
        }
        if(!string.IsNullOrEmpty(data.equippedSkill2))
        {
            var skill = _filteredSkillList.Find((x) => x.CodeName == data.equippedSkill2);
            equipment.Equip(skill, 1);
        }
           
        //장비창 로드
        equipment.LoadEquipment(data);
    }
}

