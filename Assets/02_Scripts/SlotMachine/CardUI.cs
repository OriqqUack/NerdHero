using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;

    private Effect _effect;

    public void Setup(Effect effect)
    {
        _effect = effect;
        icon.sprite = effect.Icon;
        description.text = effect.Description;
    }
}
