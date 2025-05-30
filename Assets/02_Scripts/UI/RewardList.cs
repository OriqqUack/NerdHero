using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardList : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI quantity;
    
    public void Setup(Sprite icon, int quantity)
    {
        this.icon.sprite = icon;
        if(quantity == 0)
            this.quantity.text = "";
        else
            this.quantity.text = quantity.ToString();
    }
}
