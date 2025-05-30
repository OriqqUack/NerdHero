using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image frame;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image titleImage;
    [SerializeField] private Image titleBorderImage;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI cardRarity;
    
    public Image overlayImage;
    
    private Effect _effect;

    public void Setup(Effect effect)
    {
        switch (effect.Rarity)
        {
            case EffectRarity.Common:
                frame.color = GameColors.CommonBG;
                titleImage.color = GameColors.CommonBG;
                titleBorderImage.color = GameColors.CommonGradient;
                overlayImage.color = GameColors.CommonBG;
                break;
            case EffectRarity.Rare:
                frame.color = GameColors.RareBG;
                titleImage.color = GameColors.RareBG;
                titleBorderImage.color = GameColors.RareGradient;
                overlayImage.color = GameColors.RareBG;
                break;
            case EffectRarity.Legendary:
                frame.color = GameColors.LegendaryBG;
                titleImage.color = GameColors.LegendaryBG;
                titleBorderImage.color = GameColors.LegendaryGradient;
                overlayImage.color = GameColors.LegendaryBG;
                break;
        }
        
        _effect = effect;
        icon.sprite = effect.Icon;
        titleText.text = effect.DisplayName;
        description.text = effect.Description;
        cardRarity.text = effect.Rarity.ToString();
    }
}
