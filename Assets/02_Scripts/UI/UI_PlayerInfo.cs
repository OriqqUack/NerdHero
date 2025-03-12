using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Image playerPortrait;
    [SerializeField] private Slider expSlider;

    private void Start()
    {
        GameManager.Instance.GetPlayerLevelStat().onValueChanged += UpdateLevel;
        GameManager.Instance.GetPlayerExpStat().onValueChanged += UpdateExp;
        
        UpdateLevel(GameManager.Instance.GetPlayerLevelStat(), 0 ,0);
        UpdateExp(GameManager.Instance.GetPlayerExpStat(), 0, 0);
    }

    private void UpdateLevel(Stat stat, float currentLevel, float prevLevel)
    {
        playerLevel.text = currentLevel.ToString();
    }
    
    private void UpdateExp(Stat stat, float currentLevel, float prevLevel)
    {
        expSlider.value = currentLevel / stat.MaxValue;
    }
}
