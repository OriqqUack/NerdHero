using System;
using UnityEngine;

public class Equipment : MonoSingleton<Equipment>
{
    [HideInInspector] public ItemSO weapon, helmet, armor, boots;
    [HideInInspector] public Skill skill1, skill2;
    private Stat _hpStat, _damageStat, _skillDamageStat, _defenseStat;

    public delegate void EquipmentChangeHandler(ItemSO item);
    public event EquipmentChangeHandler OnEquipmentChanged;
    
    public void Equip(ItemSO newItem)
    {
        switch (newItem.itemType)
        {
            case ItemType.Weapon:
                weapon = newItem;
                GameManager.Instance.GetPlayerDamageStat().SetBonusValue("Weapon", newItem.StatValue);
                break;
            case ItemType.Helmet:
                helmet = newItem;
                GameManager.Instance.GetPlayerDamageStat().SetBonusValue("Helmet", newItem.StatValue);
                break;
            case ItemType.Armor:
                armor = newItem;
                //GameManager.Instance.GetPlayerDefenseStat().SetBonusValue("Armor", newItem.StatValue);
                break;
            case ItemType.Boots:
                boots = newItem;
                //GameManager.Instance.GetPlayerSkillDamageStat().SetBonusValue("Boots", newItem.StatValue);
                break;
        }
        OnEquipmentChanged?.Invoke(newItem);
    }

    public void Equip(Skill newSkill, int index)
    {
        Skill previousSkill = null;

        if (index == 0)
        {
            skill1 = newSkill;
            GameManager.Instance.PlayerSkill1 = Resources.Load<Skill>("Skill/SKILL_" + newSkill.CodeName);
        }
        else
        {
            skill2 = newSkill;
            GameManager.Instance.PlayerSkill2 = Resources.Load<Skill>("Skill/SKILL_" + newSkill.CodeName);
        }
    }

    public void Unequip(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                if (weapon != null)
                {
                    weapon = null;
                    GameManager.Instance.GetPlayerDamageStat().RemoveBonusValue("Weapon");
                }
                break;
            case ItemType.Helmet:
                if (helmet != null)
                {
                    helmet = null;
                    GameManager.Instance.GetPlayerHealthStat().RemoveBonusValue("Helmet");
                }
                break;
            case ItemType.Armor:
                if (armor != null)
                {
                    armor = null;
                    //GameManager.Instance.GetPlayerDefenseStat().RemoveBonusValue("Armor");
                }
                break;
            case ItemType.Boots:
                if (boots != null)
                {
                    boots = null;
                    //GameManager.Instance.GetPlayerSkillDamageStat().RemoveBonusValue("Boots");
                }
                break;
        }

        DataManager.Instance.DataSave();
    }

    public void UnequipSkill(int index)
    {
        if (index == 0)
        {
            GameManager.Instance.PlayerSkill1 = null;
        }
        else
        {
            GameManager.Instance.PlayerSkill2 = null;
        }
        
        DataManager.Instance.DataSave();
    }
    
    public void LoadEquipment(SaveData data)
    {
        weapon = ItemDatabase.Instance.GetItemByName(data.equippedWeapon);
        if(weapon)
        {
            weapon.quantityOrLevel = data.weaponLevel;
            Equip(weapon);
        }
        helmet = ItemDatabase.Instance.GetItemByName(data.equippedHelmet);
        if (helmet)
        {
            helmet.quantityOrLevel = data.helmetLevel;
            Equip(helmet);
        }
        armor = ItemDatabase.Instance.GetItemByName(data.equippedArmor);
        if (armor)
        {
            armor.quantityOrLevel = data.armorLevel;
            Equip(armor);
        }
        boots = ItemDatabase.Instance.GetItemByName(data.equippedBoots);
        if (boots)
        {
            boots.quantityOrLevel = data.bootsLevel;
            Equip(boots);
        }
    }
}
