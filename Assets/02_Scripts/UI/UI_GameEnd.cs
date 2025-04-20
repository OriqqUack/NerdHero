using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameEnd : UiWindow
{
    [SerializeField] private GameObject panel;
    
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Slider idLevelSlider;
    [SerializeField] private Transform itemSlotParent;
    [SerializeField] private GameObject itemSlot;
    [SerializeField] private Button acceptButton;
    private void Start()
    {
        OpenUI();
        acceptButton.onClick.AddListener(() => OnClickGoMainGame());
    }

    private void OpenUI()
    {
        TimerSetting();
        WaveSetting();
        ItemSlotSetting();
    }

    private void TimerSetting()
    {
        int minutes = (int)(WaveManager.Instance.CurrentTime / 60);
        int seconds = (int)(WaveManager.Instance.CurrentTime % 60);
        timerText.text = $"TIMER : {minutes:00}:{seconds:00}";
    }

    private void WaveSetting()
    {
        waveText.text = $"WAVE : {WaveManager.Instance.CurrentWave}";
    }

    private void IDLevelSliderSetting()
    {
        
    }

    private void ItemSlotSetting()
    {
        foreach (var item in WaveManager.Instance.GetGainedItems())
        {
            var itemSlot = Instantiate(this.itemSlot, itemSlotParent).GetComponent<ItemSlot>();
            itemSlot.SetItem(item);
        }
    }

    private void OnClickGoMainGame()
    {
        //SoundManager.Instance.Play(clickSound);
        SoundManager.Instance.Clear();
        foreach (var item in WaveManager.Instance.GetGainedItems())
        {
            InventoryManager.Instance.AddItem(item);
        }
        Time.timeScale = 1;
        SceneTransitioner.Instance.StartTransitioning(SceneType.MainScene, 1, 0);
    }
    
    public override void Close()
    {
        panel.SetActive(false);
    }
}
