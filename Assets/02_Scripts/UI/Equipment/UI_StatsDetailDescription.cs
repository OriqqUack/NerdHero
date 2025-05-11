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
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Weapon");
                text.text = $"<color=#B3B5C7><size=65%>치명타 확률</size></color>\n{item.StatValue}";
                break;
            case ItemType.Helmet:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Helmet");
                text.text = $"<color=#B3B5C7><size=65%>용기 획득량</size></color>\n{item.StatValue}";
                break;
            case ItemType.Armor:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Clothes");
                text.text = $"<color=#B3B5C7><size=65%>체력</size></color>\n{item.StatValue}";
                break;
            case ItemType.Boots:
                icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Boots");
                text.text = $"<color=#B3B5C7><size=65%>회피율</size></color>\n{item.StatValue}";
                break;
        }
    }

    public void SetupSkill(Skill item)
    {
        icon.sprite = Resources.Load<Sprite>("StatIcon/Icon_Skill");
        text.text = item.Description;
    }
}
