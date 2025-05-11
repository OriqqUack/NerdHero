using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour
{
    [SerializeField] private ItemType slotType; // 무기, 헬멧, 갑옷, 신발 타입
    [SerializeField] private ItemSlot slotPrefab;
    
    private ItemSO _equippedItem;
    private Skill _skill;
    private Color _baseColor;
    private Button _unEquipBtn;
    private ItemSlot _itemSlot;
    private void Start()
    {
        _unEquipBtn = GetComponent<Button>();
        _unEquipBtn.onClick.AddListener(() => Unequip());
    }

    public void EquipItem(ItemSO newItem)
    {
        if(_itemSlot)
            DeleteSlot();
        
        _itemSlot = Instantiate(slotPrefab, transform);
        _itemSlot.GetComponent<Image>().raycastTarget = false;
        _itemSlot.GetComponent<Button>().interactable = false;
        _itemSlot.SetItem(newItem);
        _equippedItem = newItem;
    }

    public void Unequip()
    {
        if (_equippedItem != null)
        {
            Equipment.Instance.Unequip(slotType);
            DeleteSlot();
        }
    }

    public void DeleteSlot()
    {
        Destroy(_itemSlot.gameObject);
        _itemSlot = null;
    }
}