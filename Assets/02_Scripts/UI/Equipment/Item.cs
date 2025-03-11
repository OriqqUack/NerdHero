using UnityEngine;

public enum ItemType { Weapon, Helmet, Armor, Boots }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite icon; // 아이콘 저장
}