using UnityEngine;
using System;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class UI_Energy : MonoBehaviour
{
    [SerializeField] private Slider energySlider;
    [SerializeField] private TextMeshProUGUI energyCount;
    [SerializeField] private TextMeshProUGUI energyChargeLeftTimeText;
    private EnergyManager energyManager;

    private void Start()
    {
        energyManager = Managers.EnergyManager;
        energyManager.OnEnergyChange += EnergySliderValueChanged;
        UIUpdateEnergy();
        energySlider.value = energyManager.CurrentEnergy / (float)energyManager.maxEnergy;
    }

    private void Update()
    {
        if (energyManager.CurrentEnergy >= energyManager.maxEnergy) return;
        UIUpdateEnergy();
    }

    private void EnergySliderValueChanged(int energy)
    {
        energySlider.value = energy / (float)energyManager.maxEnergy;
    }

    private void UIUpdateEnergy()
    {
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

    #if UNITY_EDITOR
    [ContextMenu("Init Energy Count")]
    public void InitEnergyCount()
    {
        energyManager.ClearEnergy();
    }
    #endif
}
