using System;
using TMPro;
using UnityEngine;

public class UI_Currency : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gold;

    private Stat _playerMoney;

    private void Start()
    {
        GameManager.Instance.GetPlayerGoldStat().onValueChanged += UpdateGold;
    }
    
    private void UpdateGold(Stat stat, float currentLevel, float prevLevel)
    {
        gold.text = currentLevel.ToString();
    }
}
