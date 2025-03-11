using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameEnd : UI_Popup
{
    [SerializeField] private GameObject panel;
    
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Slider idLevelSlider;
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private GameObject itemSlot;

    private void Start()
    {
        WaveManager.Instance.OnWaveEnd += OpenUI;
        panel.SetActive(false);
    }

    private void OpenUI()
    {
        TimerSetting();
        WaveSetting();
        ItemSlotSetting();
        
        panel.SetActive(true);
    }

    private void TimerSetting()
    {
        int minutes = (int)(WaveManager.Instance.CurrentTime / 60);
        int seconds = (int)(WaveManager.Instance.CurrentTime % 60);
        timerText.text = $"TIMER : {minutes:00}:{seconds:00}";
    }

    private void WaveSetting()
    {
        waveText.text = $"WAVE : {WaveManager.Instance.CurrentWave + 1}";
    }

    private void IDLevelSliderSetting()
    {
        
    }

    private void ItemSlotSetting()
    {
        foreach (var kvp in WaveManager.Instance.GainedItemsList)
        {
            var item = Instantiate(itemSlot, itemSlotParent);
            //item.transform.Find("Icon").GetComponent<Image>().sprite = kvp.Key.Icon;
            item.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = kvp.Value.ToString();
        }
    }

    public override void Close()
    {
        Time.timeScale = 1;
        panel.SetActive(false);
    }
}
