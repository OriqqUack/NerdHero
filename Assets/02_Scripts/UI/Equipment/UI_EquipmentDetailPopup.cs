using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EquipmentDetailPopup : UiWindow
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon; // 아이템 아이콘
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI itemLevelText;
    [SerializeField] private Transform contents;
    [SerializeField] private Transform buttonContents;
    [SerializeField] private UI_StatsDetailDescription statDetailPrefab;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button dimed;

    private ItemSO _currentItem;
    private Skill _currentSkill;
    private ItemRarity _itemRarity;

    public void SetupItem(ItemSO item)
    {
        foreach (Transform child in contents) Destroy(child.gameObject);
        foreach (Transform child in buttonContents) Destroy(child.gameObject);
        
        _currentItem = item;
        _itemRarity = item.itemRarity;
        icon.sprite = item.icon;
        icon.enabled = true;
        
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
        
        UI_StatsDetailDescription statPrefab = Instantiate(statDetailPrefab, contents);
        statPrefab.Setup(item);
        
        Button cloneBtn = Instantiate(equipButton, buttonContents);
        cloneBtn.onClick.AddListener(() => EquipItem(_currentItem));
        cloneBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "장착";
    }

    public void SetupSkill(Skill skill)
    {
        foreach (Transform child in contents) Destroy(child.gameObject);
        foreach (Transform child in buttonContents) Destroy(child.gameObject);

        _currentSkill = skill;
        icon.sprite = skill.Icon;
        icon.enabled = true;
        
        backgroundImage.sprite = Resources.Load<Sprite>("ItemRarity/SkillFrame");
        
        quantityText.gameObject.SetActive(false);
        itemLevelText.gameObject.SetActive(true);
        itemLevelText.text = $"Lv. " + skill.Level.ToString();

        UI_StatsDetailDescription statPrefab = Instantiate(statDetailPrefab, contents);
        statPrefab.SetupSkill(skill);
        
        Button cloneBtn = Instantiate(equipButton, buttonContents);
        cloneBtn.onClick.AddListener(() => EquipSkill(_currentSkill, 0));
        cloneBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "스킬1";
        Button cloneBtn1 = Instantiate(equipButton, buttonContents);
        cloneBtn1.onClick.AddListener(() => EquipSkill(_currentSkill, 1));
        cloneBtn1.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = "스킬2";
    }
    
    public void EquipItem(ItemSO item)
    {
        if (item.itemType == ItemType.Stuff) return;
        Equipment.Instance.Equip(item);
        DataManager.Instance.DataSave();
    }

    public void EquipSkill(Skill skill, int index)
    {
        Equipment.Instance.Equip(skill, index);
        DataManager.Instance.DataSave();
    }
}
