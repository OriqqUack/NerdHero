using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UI_Energy : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI energyCount;
    [SerializeField] private TextMeshProUGUI energyChargeLeftTimeText;
    private EnergyManager energyManager;

    private void Start()
    {
        energyManager = EnergyManager.Instance;
    }

    private void Update()
    {
        if (energyManager.CurrentEnergy >= energyManager.maxEnergy) return;
        energyCount.text = $"{energyManager.CurrentEnergy} / {energyManager.maxEnergy}";
        TimeSpan timeLeft = TimeSpan.FromMinutes(energyManager.chargeIntervalMinutes) - energyManager.PassedTime;
        timeLeft = timeLeft < TimeSpan.Zero ? TimeSpan.Zero : timeLeft;

        energyChargeLeftTimeText.text = $"{timeLeft.Minutes:00}:{timeLeft.Seconds:00}";
    }

    [ContextMenu("Use Energy")]
    public void UseEnergy()
    {
        energyManager.UseEnergy(5);
    }

    [ContextMenu("Init Energy Count")]
    public void InitEnergyCount()
    {
        energyManager.ClearEnergy();
    }
}
