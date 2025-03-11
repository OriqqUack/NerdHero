using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon; // 아이템 아이콘
    public Button equipButton; // 장착 버튼
    private ItemSO currentItem;

    public void SetItem(ItemSO item)
    {
        currentItem = item;
        icon.sprite = item.icon;
        icon.enabled = true;
        equipButton.gameObject.SetActive(true);
    }

    public void ClearSlot()
    {
        currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
        equipButton.gameObject.SetActive(false);
    }

    public void OnEquipButtonClick()
    {
        if (currentItem != null)
        {
            Inventory.instance.EquipItem(currentItem);
        }
    }

    private void Start()
    {
        equipButton.onClick.AddListener(OnEquipButtonClick);
    }
}