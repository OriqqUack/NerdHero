using System;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoSingleton<EquipmentPanel>
{
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        closeButton.onClick.AddListener(() => Close());
    }

    public void Show()
    {
        equipmentPanel.SetActive(true);
    }

    public void Close()
    {
        equipmentPanel.SetActive(false);
    }
}
