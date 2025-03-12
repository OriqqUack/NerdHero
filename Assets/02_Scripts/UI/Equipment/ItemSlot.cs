using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon; // 아이템 아이콘
    private ItemSO currentItem;
    private Button button;
    public void SetItem(ItemSO item)
    {
        currentItem = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Inventory.instance.OnClickItem(item));
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }
}