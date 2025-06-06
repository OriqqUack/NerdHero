using UnityEngine;

public enum ItemType { Weapon, Helmet, Armor, Boots, Stuff, Skill}
public enum ItemRarity { Common, Rare, Legendary }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public int itemCode;
    public ItemType itemType;
    public ItemRarity itemRarity;
    public Sprite icon;
    public int quantityOrLevel;
    public float statValue;
    
    public float StatValue => statValue * quantityOrLevel;
    
    [HideInInspector] public bool isOld;
}