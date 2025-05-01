using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Color imageColor;
    [SerializeField] private GameObject textGo;

    private Color _imageColor;

    private void Start()
    {
        _imageColor = image.color;
    }

    public void Setup()
    {
        image.color = imageColor;
        textGo.SetActive(true);
    }

    public void Reset()
    {
        image.color = _imageColor;
        textGo.SetActive(false);
    }
}
