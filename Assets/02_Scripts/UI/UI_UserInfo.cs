using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo : MonoSingleton<UI_UserInfo>
{
    [SerializeField] private Slider hpSlider; 
    [SerializeField] private Slider energySlider; 
    [SerializeField] private Slider expSlider;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private Stat energyStat;
    [SerializeField] private Stat expStat;
    [SerializeField] private Stat levelStat;
    private Stat hpStat;
    
    private Stats _playerStats;
    private float _maxHp;
    
    private void Start()
    {
        _playerStats = GameManager.Instance.GetComponent<Stats>();
        energyStat = _playerStats.GetStat(energyStat);
        expStat = _playerStats.GetStat(expStat);
        hpStat = _playerStats.HPStat;
        levelStat = _playerStats.GetStat(levelStat);
        
        hpStat.onValueChanged += HpChangeEvent;
        energyStat.onValueChanged += EnergyChangeEvent;
        expStat.onValueChanged += ExpChangeEvent;
        levelStat.onValueChanged += LevelChangeEvent;
        
        _maxHp = hpStat.Value;
        
        hpSlider.value = hpStat.Value / _maxHp;
        energySlider.value = energyStat.Value / energyStat.MaxValue;
        expSlider.value = expStat.Value / expStat.MaxValue;
        hpText.text = $"{hpStat.Value} / {_maxHp}";
        levelText.text = levelStat.Value.ToString();
    }

    private void HpChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        hpText.text = $"{hpStat.Value} / {_maxHp}";
        hpSlider.value = currentValue/_maxHp;
    }
    
    private void EnergyChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        energySlider.value = currentValue/energyStat.MaxValue;
    }
    
    private void ExpChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        expSlider.value = currentValue/expStat.MaxValue;
        if (expStat.IsMax)
        {
            _playerStats.SetDefaultValue(stat, currentValue - expStat.MaxValue);
            _playerStats.IncreaseDefaultValue(levelStat, 1.0f);
        }
    }

    private void LevelChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        levelText.text = $"{levelStat.Value}";
    }
}
