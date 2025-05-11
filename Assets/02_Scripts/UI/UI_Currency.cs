using System;
using TMPro;
using UnityEngine;

public class UI_Currency : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI gemText;

    private void Start()
    {
        GameManager.Instance.OnGoldChanged += UpdateGold;
        GameManager.Instance.OnGemChanged += UpdateGem;
        
        goldText.text = GameManager.Instance.Gold.ToString();
        gemText.text = GameManager.Instance.Gem.ToString();
    }
    
    private void UpdateGold(int gold)
    {
        goldText.text = gold.ToString();
    }
    
    private void UpdateGem(int gem)
    {
        gemText.text = gem.ToString();
    }
}
