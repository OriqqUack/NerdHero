
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoSingleton<UI_Inventory>
{
    [Header("UI Elements")]
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private TextMeshProUGUI inventoryAmount;
    [SerializeField] private Transform sortArrowSprite;
    [SerializeField] private TMP_Dropdown dropdown;

    [Space(10)] [Header("Buttons")]
    //[SerializeField] private Button sortButton;
    [SerializeField] private Button weaponFilterBtn;
    [SerializeField] private Button armorFilterBtn;
    [SerializeField] private Button allFilterBtn;

    private bool isWeaponFilter = false;
    private bool isArmorFilter = false;
    private InventoryTab weaponImage;
    private InventoryTab armorImage;
    private InventoryTab allImage;

    private int currentDropdownIndex;
    private bool isAcending;
    private void Start()
    {
        //sortButton.onClick.AddListener(() => SortItem());
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        weaponImage = weaponFilterBtn.GetComponent<InventoryTab>();
        armorImage = armorFilterBtn.GetComponent<InventoryTab>();
        allImage = allFilterBtn.GetComponent<InventoryTab>();
        
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
        List<ItemSO> filteredItems = Managers.InventoryManager.GetItems();

        if (isWeaponFilter)
        {
            filteredItems = filteredItems.FindAll(item => item.itemType == ItemType.Weapon);
            weaponImage.Setup();
            armorImage.Clear();
            allImage.Clear();
        }
        else if (isArmorFilter)
        {
            filteredItems = filteredItems.FindAll(item =>
                item.itemType == ItemType.Helmet || item.itemType == ItemType.Armor || item.itemType == ItemType.Boots);
            weaponImage.Clear();
            armorImage.Setup();
            allImage.Clear();
        }
        else
        {
            weaponImage.Clear();
            armorImage.Clear();
            allImage.Setup();
        }

        foreach (ItemSO item in filteredItems)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryPanel);
            slotObj.GetComponent<ItemSlot>().SetItem(item);
        }

        //Skill
        /*foreach (Skill skill in filteredSkills)
        {
            GameObject slotObj = Instantiate(inventorySkillSlotPrefab, inventoryPanel);
            slotObj.GetComponent<InventorySkillSlot>().SetItem(skill);
        }*/

        inventoryAmount.text = $"<color=#32bbff>{filteredItems.Count}</color> / 120";
    }
    
    #region ButtonEvent
    void OnDropdownValueChanged(int index)
    {
        string selectedText = dropdown.options[index].text;
        currentDropdownIndex = index;
        switch (selectedText)
        {
            case "레벨":
                SortItem(true, isAcending);
                break;
            case "등급":
                SortItem(false, isAcending);
                break;
        }
    }

    public void SortItem()
    {
        isAcending = !isAcending;
        
        if(isAcending) sortArrowSprite.rotation = Quaternion.Euler(0, 0, 180);
        else sortArrowSprite.rotation = Quaternion.Euler(0, 0, 0);
        
        OnDropdownValueChanged(currentDropdownIndex);
    }
    
    private void SortItem(bool sortByLevel, bool ascending)
    {
        var items = Managers.InventoryManager.GetItems();

        if (sortByLevel)
        {
            if (ascending)
                items.Sort((a, b) => a.quantityOrLevel.CompareTo(b.quantityOrLevel));
            else
                items.Sort((a, b) => b.quantityOrLevel.CompareTo(a.quantityOrLevel));
        }
        else
        {
            if (ascending)
                items.Sort((a, b) => a.itemRarity.CompareTo(b.itemRarity));
            else
                items.Sort((a, b) => b.itemRarity.CompareTo(a.itemRarity));
        }

        UpdateInventoryUI();
    }
    #endregion

    [ContextMenu("Update Inventory UI")]
    public void UpdatUI()
    {
        UpdateInventoryUI();
    }
}