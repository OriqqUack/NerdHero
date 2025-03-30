
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
    [SerializeField] private Sprite focusImage;
    [SerializeField] private Sprite defaultImage;
    [SerializeField] private Transform sortArrowSprite;
    [SerializeField] private TMP_Dropdown dropdown;

    [Space(10)] [Header("Buttons")]
    //[SerializeField] private Button sortButton;
    [SerializeField] private Button weaponFilterBtn;
    [SerializeField] private Button armorFilterBtn;
    [SerializeField] private Button allFilterBtn;

    private bool isWeaponFilter = false;
    private bool isArmorFilter = false;
    private Image weaponImage;
    private Image armorImage;
    private Image allImage;

    private int currentDropdownIndex;
    private bool isAcending;
    private void Start()
    {
        //sortButton.onClick.AddListener(() => SortItem());
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        weaponImage = weaponFilterBtn.GetComponent<Image>();
        armorImage = armorFilterBtn.GetComponent<Image>();
        allImage = allFilterBtn.GetComponent<Image>();
        
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
        {
            filteredItems = filteredItems.FindAll(item => item.itemType == ItemType.Weapon);
            weaponImage.sprite = focusImage;
            armorImage.sprite = defaultImage;
            allImage.sprite = defaultImage;
        }
        else if (isArmorFilter)
        {
            filteredItems = filteredItems.FindAll(item =>
                item.itemType == ItemType.Helmet || item.itemType == ItemType.Armor || item.itemType == ItemType.Boots);
            weaponImage.sprite = defaultImage;
            armorImage.sprite = focusImage;
            allImage.sprite = defaultImage;
        }
        else
        {
            weaponImage.sprite = defaultImage;
            armorImage.sprite = defaultImage;
            allImage.sprite = focusImage;
        }

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
        var items = InventoryManager.Instance.GetItems();

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