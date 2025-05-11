using System;
using UnityEngine;

public class Equipment : MonoSingleton<Equipment>, ISaveable
{
    [HideInInspector] public ItemSO weapon, helmet, armor, boots;
    private Stat _hpStat, _damageStat, _skillDamageStat, _defenseStat;

    public delegate void EquipmentChangeHandler(ItemSO item);
    public event EquipmentChangeHandler OnEquipmentChanged;

    public void Equip(ItemSO newItem)
    {
        switch (newItem.itemType)
        {
            case ItemType.Weapon:
                if (weapon == newItem) return;
                weapon = newItem;
                GameManager.Instance.GetCriticalPerStat().SetBonusValue("Weapon", newItem.StatValue);
                break;
            case ItemType.Helmet:
                if (helmet == newItem) return;
                helmet = newItem;
                GameManager.Instance.GetEnergyAmountStat().SetBonusValue("Helmet", newItem.StatValue);
                break;
            case ItemType.Armor:
                if (armor == newItem) return;
                armor = newItem;
                GameManager.Instance.GetPlayerHealthStat().SetBonusValue("Armor", newItem.StatValue);
                GameManager.Instance.SetHpToMaxHp();
                break;
            case ItemType.Boots:
                if (boots == newItem) return;
                boots = newItem;
                GameManager.Instance.GetDodgePerStat().SetBonusValue("Boots", newItem.StatValue);
                break;
        }
        OnEquipmentChanged?.Invoke(newItem);
    }

    public void Unequip(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                if (weapon != null)
                {
                    weapon = null;
                    GameManager.Instance.GetCriticalPerStat().RemoveBonusValue("Weapon");
                }
                break;
            case ItemType.Helmet:
                if (helmet != null)
                {
                    helmet = null;
                    GameManager.Instance.GetEnergyAmountStat().RemoveBonusValue("Helmet");
                }
                break;
            case ItemType.Armor:
                if (armor != null)
                {
                    armor = null;
                    GameManager.Instance.GetPlayerHealthStat().RemoveBonusValue("Armor");
                }
                break;
            case ItemType.Boots:
                if (boots != null)
                {
                    boots = null;
                    GameManager.Instance.GetDodgePerStat().RemoveBonusValue("Boots");
                }
                break;
        }
        //TODO 장비 세이브
    }

    
    public void Load(GameData gameData)
    {
        weapon = ItemDatabase.Instance.GetItemById(gameData.equippedWeapon);
        if(weapon)
        {
            weapon.quantityOrLevel = 1;
            Equip(weapon);
        }
        helmet = ItemDatabase.Instance.GetItemById(gameData.equippedHelmet);
        if (helmet)
        {
            helmet.quantityOrLevel = 1;
            Equip(helmet);
        }
        armor = ItemDatabase.Instance.GetItemById(gameData.equippedArmor);
        if (armor)
        {
            armor.quantityOrLevel = 1;
            Equip(armor);
        }
        boots = ItemDatabase.Instance.GetItemById(gameData.equippedBoots);
        if (boots)
        {
            boots.quantityOrLevel = 1;
            Equip(boots);
        }
    }

    public void Save(GameData data)
    {
    }
}
