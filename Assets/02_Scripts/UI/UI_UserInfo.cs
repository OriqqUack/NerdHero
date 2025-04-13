using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UserInfo : MonoSingleton<UI_UserInfo>
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider hpFollowSlider;
    [SerializeField] private Slider energySlider; 
    [SerializeField] private Slider energyFollowSlider;
    [SerializeField] private Slider expSlider;

    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI waveText;
    
    [SerializeField] private Stat energyStat;
    [SerializeField] private Stat expStat;
    [SerializeField] private Stat levelStat;
    private Stat hpStat;
    
    private Stats _playerStats;
    
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
        WaveManager.Instance.OnWaveChange += WaveChangeEvent;
        
        hpSlider.value = hpStat.Value / hpStat.MaxValue;
        energySlider.value = energyStat.Value / energyStat.MaxValue;
        expSlider.value = expStat.Value / expStat.MaxValue;
        hpText.text = $"{hpStat.Value} / {hpStat.MaxValue}";
        energyText.text = $"{energyStat.Value} / {energyStat.MaxValue}";
        levelText.text = levelStat.Value.ToString();
    }

    private void WaveChangeEvent(int waveIndex)
    {
        waveText.text = $"WAVE : {waveIndex}";
    }

    private void HpChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        UpdateStatViewLerp(stat, hpSlider, hpFollowSlider, hpText);
    }
    
    private void UpdateStatViewLerp(Stat stat, Slider statFillAmount, Slider hpFollowFillAmount = null, TextMeshProUGUI statText = null)
    {
        float value = stat.Value / stat.MaxValue;
        float duration = 0.3f;
    
        // 먼저 바로 채워지는 슬라이더
        statFillAmount.DOValue(value, duration).SetEase(Ease.OutQuad);

        if(hpFollowFillAmount != null)
        {
            // 살짝 딜레이 후 따라오는 슬라이더
            DOVirtual.DelayedCall(0.2f, () => { hpFollowFillAmount.DOValue(value, duration).SetEase(Ease.OutQuad); });
        }

        if(statText != null)
        {
            statText.text = $"{Mathf.RoundToInt(stat.Value)} / {stat.MaxValue}";
        }
    }
    
    private void EnergyChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        UpdateStatViewLerp(stat, energySlider, energyFollowSlider, energyText);
    }
    
    private void ExpChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        float value = stat.Value / stat.MaxValue;
        float duration = 0.3f;
    
        // 먼저 바로 채워지는 슬라이더
        expSlider.DOValue(value, duration).SetEase(Ease.OutQuad).OnComplete(() => 
        { 
            if (expStat.IsMax)
            {
                _playerStats.SetDefaultValue(stat, currentValue - expStat.MaxValue);
                _playerStats.IncreaseDefaultValue(levelStat, 1.0f); 
            }
        });
        
    }

    private void LevelChangeEvent(Stat stat, float currentValue, float prevValue)
    {
        levelText.text = $"{levelStat.Value}";
    }
}
