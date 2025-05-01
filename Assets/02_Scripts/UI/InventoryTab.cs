using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private GameObject glowImage;
    [SerializeField] private GameObject glowLine;
    [SerializeField] private Image border;

    private Color _baseColor;
    private Color _baseBorderColor;

    private void Awake()
    {
        _baseColor = bg.color;
        _baseBorderColor = border.color;
    }

    public void Setup()
    {
        bg.color = GameColors.SelectedInventoryTabColor;
        border.color = GameColors.SelectedInventoryBorderColor;
        glowImage.SetActive(true);
        glowLine.SetActive(true);
    }

    public void Clear()
    {
        bg.color = _baseColor;
        border.color = _baseBorderColor;
        glowImage.SetActive(false);
        glowLine.SetActive(false);
    }
}
