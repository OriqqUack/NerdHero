using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image gradientImage;
    [SerializeField] private Image icon; // 아이템 아이콘
    [SerializeField] private GameObject categoryDot;
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

        switch(item.itemRarity)
        {
            case ItemRarity.Common:
                backgroundImage.color = GameColors.CommonBG;
                gradientImage.color = GameColors.CommonGradient;
                break;
            case ItemRarity.Rare:
                backgroundImage.color = GameColors.RareBG;
                gradientImage.color = GameColors.RareGradient;
                break;
            case ItemRarity.Legendary:
                backgroundImage.color = GameColors.LegendaryBG;
                gradientImage.color = GameColors.LegendaryGradient;
                break;
        }
        
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
        _button.onClick.AddListener(() => OpenDetailPopup());
    }

    private void OpenDetailPopup()
    {
        _currentItem.isOld = true;
        UI_MainScene.Instance.OpenEquipmentDetailPopup(_currentItem);
    }
    
    public void ClearSlot()
    {
        _currentItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }
}