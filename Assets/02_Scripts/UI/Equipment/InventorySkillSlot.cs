using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySkillSlot : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image icon; // 아이템 아이콘
    [SerializeField] private TextMeshProUGUI itemLevelText;
    
    private Skill _currentSkill;
    private Button _button;
    
    public void SetItem(Skill skill)
    {
        _currentSkill = skill;
        icon.sprite = skill.Icon;
        icon.enabled = true;

        itemLevelText.gameObject.SetActive(true);
        itemLevelText.text = $"Lv. " + skill.Level.ToString();
        
        _button = GetComponent<Button>();
        //_button.onClick.AddListener(() => UI_EquipmentDetailPopup.Instance.SetupSkill(skill));
    }
}
