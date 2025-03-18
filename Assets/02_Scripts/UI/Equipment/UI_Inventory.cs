
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoSingleton<UI_Inventory>
{
    [Header("UI Elements")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private GameObject inventorySkillSlotPrefab;
    [SerializeField] private TextMeshProUGUI inventoryAmount;
    [SerializeField] private Transform focusImage;

    [Space(10)] [Header("Buttons")]
    [SerializeField] private Button sortButton;
    [SerializeField] private Button weaponFilterBtn;
    [SerializeField] private Button armorFilterBtn;
    [SerializeField] private Button allFilterBtn;

    private bool isWeaponFilter = false;
    private bool isArmorFilter = false;

    private void Start()
    {
        sortButton.onClick.AddListener(() => SortItem());
        weaponFilterBtn.onClick.AddListener(() => ApplyFilter(true, false));
        armorFilterBtn.onClick.AddListener(() => ApplyFilter(false, true));
        allFilterBtn.onClick.AddListener(() => ApplyFilter(false, false));
        UpdateInventoryUI();
    }

    private void ApplyFilter(bool isWeapon, bool isArmor)
    {
        isWeaponFilter = isWeapon;
        isArmorFilter = isArmor;
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        foreach (Transform child in inventoryPanel) Destroy(child.gameObject);
        List<ItemSO> filteredItems = InventoryManager.Instance.GetItems();
        List<Skill> filteredSkills = InventoryManager.Instance.GetSkills();

        if (isWeaponFilter)
            filteredItems = filteredItems.FindAll(item => item.itemType == ItemType.Weapon);
        else if (isArmorFilter)
            filteredItems = filteredItems.FindAll(item => item.itemType == ItemType.Helmet || item.itemType == ItemType.Armor || item.itemType == ItemType.Boots);

        foreach (ItemSO item in filteredItems)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel);
            slotObj.GetComponent<ItemSlot>().SetItem(item);
        }

        foreach (Skill skill in filteredSkills)
        {
            GameObject slotObj = Instantiate(inventorySkillSlotPrefab, inventoryPanel);
            slotObj.GetComponent<InventorySkillSlot>().SetItem(skill);
        }

        inventoryAmount.text = $"<color=#32bbff>{filteredItems.Count + filteredSkills.Count}</color> / 120";
    }
    
    #region ButtonEvent
    private void SortItem()
    {
        InventoryManager.Instance.GetItems().Sort((a, b) => a.itemCode.CompareTo(b.itemCode));
        UpdateInventoryUI();
    }
    #endregion
}