using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatsDetailDescription : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI text;
    
    public void Setup(ItemSO item)
    {
        switch (item.itemType)
        {
            case ItemType.Weapon:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Damage");
                text.text = $"<color=#B3B5C7><size=65%>공격력</size></color>\n{item.StatValue}";
                break;
            case ItemType.Helmet:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Health");
                text.text = $"<color=#B3B5C7><size=65%>체력</size></color>\n{item.StatValue}";
                break;
            case ItemType.Armor:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Defense");
                text.text = $"<color=#B3B5C7><size=65%>방어력</size></color>\n{item.StatValue}";
                break;
            case ItemType.Boots:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_SkillDamage");
                text.text = $"<color=#B3B5C7><size=65%>스킬 데미지</size></color>\n{item.StatValue}";
                break;
        }
    }

    public void SetupSkill(Skill item)
    {
        icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Skill");
        text.text = item.Description;
    }
}
