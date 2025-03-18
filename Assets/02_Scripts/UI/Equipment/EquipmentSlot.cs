using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private ItemType slotType; // 무기, 헬멧, 갑옷, 신발 타입
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon; // 슬롯에 표시될 아이콘
    [SerializeField] private TextMeshProUGUI itemLevelText;
    [SerializeField] private int skillIndex;
    
    private ItemSO _equippedItem;
    private Skill _skill;
    private Color _baseColor;
    private Button _unEquipBtn;
    private void Start()
    {
        _baseColor = icon.color;
        _unEquipBtn = GetComponent<Button>();
        if(slotType == ItemType.Skill)
            _unEquipBtn.onClick.AddListener(() => UnequipSkill(skillIndex));
        else
            _unEquipBtn.onClick.AddListener(() => Unequip());
    }

    public void EquipItem(ItemSO newItem)
    {
        _equippedItem = newItem;
        icon.sprite = newItem.icon;
        icon.color = Color.white;
        
        backgroundImage.sprite = Resources.Load<Sprite>("ItemRarity/ItemFrame_" + newItem.itemRarity.ToString());
        itemLevelText.gameObject.SetActive(true);
        itemLevelText.text = $"Lv. " + newItem.quantityOrLevel.ToString();
    }
    
    public void EquipItem(Skill newSKill)
    {
        _skill = newSKill;

        icon.sprite = newSKill.Icon;
        itemLevelText.gameObject.SetActive(true);
        itemLevelText.text = $"Lv. " + newSKill.Level.ToString();
    }

    public void Unequip()
    {
        if (_equippedItem != null)
        {
            Equipment.Instance.Unequip(slotType);
            UpdateSlot(null);
        }
    }

    public void UnequipSkill(int index)
    {
        if (_skill != null)
        {
            Equipment.Instance.UnequipSkill(index);
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
        else if (slotType == ItemType.Skill)
        {
            icon.color = _baseColor;
            icon.sprite = Resources.Load<Sprite>("ItemBaseIcon/Icon_"+slotType.ToString());
            itemLevelText.gameObject.SetActive(false);
        }
        else
        {
            icon.color = _baseColor;
            icon.sprite = Resources.Load<Sprite>("ItemBaseIcon/Icon_"+slotType.ToString());
            backgroundImage.sprite = Resources.Load<Sprite>("ItemRarity/ItemFrame_Base");
            itemLevelText.gameObject.SetActive(false);
        }
    }
}