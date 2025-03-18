using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon; // 아이템 아이콘
    [SerializeField] private GameObject alertDot;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI itemLevelText;
    
    private ItemSO _currentItem;
    private Button _button;
    private ItemRarity _itemRarity;
    
    public void SetItem(ItemSO item)
    {
        _currentItem = item;
        _itemRarity = item.itemRarity;
        icon.sprite = item.icon;
        icon.enabled = true;

        if(item.isOld)
            alertDot.SetActive(false);
        
        backgroundImage.sprite = Resources.Load<Sprite>("ItemRarity/ItemFrame_" + item.itemRarity.ToString());

        if (item.itemType == ItemType.Stuff)
        {
            itemLevelText.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(true);
            quantityText.text = item.quantityOrLevel.ToString();
        }
        else
        {
            quantityText.gameObject.SetActive(false);
            itemLevelText.gameObject.SetActive(true);
            itemLevelText.text = $"Lv. " + item.quantityOrLevel.ToString();
        }
        
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => DisappearAlertDot());
    }

    private void DisappearAlertDot()
    {
        _currentItem.isOld = true;
        alertDot.SetActive(false);
        UI_EquipmentDetailPopup.Instance.SetupItem(_currentItem);
    }
    
    public void ClearSlot()
    {
        _currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }
}