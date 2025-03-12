using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    public ItemType slotType; // 무기, 헬멧, 갑옷, 신발 타입
    public Image icon; // 슬롯에 표시될 아이콘

    private ItemSO equippedItem;

    private void Start()
    {
        UpdateSlot(null); // 초기화
    }

    public void EquipItem(ItemSO newItem)
    {
        equippedItem = newItem;
        icon.sprite = newItem.icon;
        icon.enabled = true;
    }

    public void Unequip()
    {
        if (equippedItem != null)
        {
            Inventory.instance.AddItem(equippedItem);
            Equipment.instance.Unequip(slotType);
            UpdateSlot(null);
        }
    }

    public void UpdateSlot(ItemSO item)
    {
        if (item != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }
}