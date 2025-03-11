using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<string> inventoryItems = new List<string>(); // 인벤토리 아이템 (이름 저장)
    public string equippedWeapon;
    public string equippedHelmet;
    public string equippedArmor;
    public string equippedBoots;
}