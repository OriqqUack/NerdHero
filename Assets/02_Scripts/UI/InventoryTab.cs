using System;
using UnityEngine;
using UnityEngine.UI;

public class InventoryTab : MonoBehaviour
{
    [SerializeField] private Image bg;
    [SerializeField] private GameObject glowImage;
    [SerializeField] private GameObject glowLine;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Image border;
    [SerializeField] private Color borderColor;

    private Color _baseColor;
    private Color _baseBorderColor;

    private void Start()
    {
        _baseColor = bg.color;
        _baseBorderColor = border.color;
    }

    public void Setup()
    {
        bg.color = selectedColor;
        border.color = borderColor;
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
