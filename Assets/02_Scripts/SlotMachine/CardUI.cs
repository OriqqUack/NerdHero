using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image frame;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;

    private Effect _effect;

    public void Setup(Effect effect)
    {
        frame.sprite = Resources.Load<Sprite>($"Sprites/CardFrame/CardFrame_{effect.Rarity.ToString()}");
        _effect = effect;
        icon.sprite = effect.Icon;
        description.text = effect.Description;
    }
}
