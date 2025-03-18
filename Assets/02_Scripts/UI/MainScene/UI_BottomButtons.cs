using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_BottomButtons : MonoBehaviour
{
    [SerializeField] private Button heroBtn, equipBtn, artifactBtn, upgradeBtn, shopBtn;
    [SerializeField] private GameObject equipPanel;

    private void Start()
    {
        equipBtn.onClick.AddListener(() => EquipmentPanel.Instance.Show());
    }
}
