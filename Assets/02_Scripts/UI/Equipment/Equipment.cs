using UnityEngine;

public class Equipment : MonoBehaviour
{
    public static Equipment instance;

    [HideInInspector] public ItemSO weapon, helmet, armor, boots;
    public EquipmentSlot weaponSlot, helmetSlot, armorSlot, bootsSlot;

    private void Awake()
    {
        instance = this;
    }

    public void Equip(ItemSO newItem)
    {
        ItemSO previousItem = null;

        switch (newItem.itemType)
        {
            case ItemType.Weapon:
                previousItem = weapon;
                weapon = newItem;
                weaponSlot.EquipItem(newItem);
                break;
            case ItemType.Helmet:
                previousItem = helmet;
                helmet = newItem;
                helmetSlot.EquipItem(newItem);
                break;
            case ItemType.Armor:
                previousItem = armor;
                armor = newItem;
                armorSlot.EquipItem(newItem);
                break;
            case ItemType.Boots:
                previousItem = boots;
                boots = newItem;
                bootsSlot.EquipItem(newItem);
                break;
        }

        Inventory.instance.SaveInventory();
    }

    public void Unequip(ItemType type)
    {
        switch (type)
        {
            case ItemType.Weapon:
                if (weapon != null)
                {
                    Inventory.instance.AddItem(weapon);
                    weapon = null;
                    weaponSlot.UpdateSlot(null);
                }
                break;
            case ItemType.Helmet:
                if (helmet != null)
                {
                    Inventory.instance.AddItem(helmet);
                    helmet = null;
                    helmetSlot.UpdateSlot(null);
                }
                break;
            case ItemType.Armor:
                if (armor != null)
                {
                    Inventory.instance.AddItem(armor);
                    armor = null;
                    armorSlot.UpdateSlot(null);
                }
                break;
            case ItemType.Boots:
                if (boots != null)
                {
                    Inventory.instance.AddItem(boots);
                    boots = null;
                    bootsSlot.UpdateSlot(null);
                }
                break;
        }

        Inventory.instance.SaveInventory();
    }
    
    public void LoadEquipment(SaveData data)
    {
        weapon = ItemDatabase.instance.GetItemByName(data.equippedWeapon);
        helmet = ItemDatabase.instance.GetItemByName(data.equippedHelmet);
        armor = ItemDatabase.instance.GetItemByName(data.equippedArmor);
        boots = ItemDatabase.instance.GetItemByName(data.equippedBoots);
    }
}
