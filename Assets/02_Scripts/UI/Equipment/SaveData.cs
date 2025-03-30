using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int energyCount;
    public string energyLastCharge;
    
    public List<string> inventoryItems = new List<string>(); // 인벤토리 아이템 (이름 저장)
    public List<int> quantityOrLevel = new List<int>();
    public List<string> skills = new List<string>();
    public List<int> skillLevels = new List<int>();
    public string equippedWeapon;
    public int weaponLevel;
    public string equippedHelmet;
    public int helmetLevel;
    public string equippedArmor;
    public int armorLevel;
    public string equippedBoots;
    public int bootsLevel;
    public string equippedSkill1;
    public string equippedSkill2;

    public SaveData()
    {
        energyCount = 30;
    }
}