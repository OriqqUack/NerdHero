using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerLevel;
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private TextMeshProUGUI playerNickname;
    [SerializeField] private Slider expSlider;

    private void Start()
    {
        GameManager.Instance.OnPlayerLevelChanged += UpdateLevel;
        GameManager.Instance.OnExpChanged += UpdateExp;
        
        UpdateLevel(GameManager.Instance.PlayerLevelRaw);
        UpdateExp(GameManager.Instance.CurrentExp);
    }

    public void UpdateNickName()
    {
        playerNickname.text = string.IsNullOrEmpty(UserInfo.Data.nickName) ? UserInfo.Data.gamerId : UserInfo.Data.nickName;
    }

    private void UpdateLevel(int level)
    {
        playerLevel.text = level.ToString();
    }
    
    private void UpdateExp(int exp)
    {
        expSlider.value = exp / 100f;
    }
}
