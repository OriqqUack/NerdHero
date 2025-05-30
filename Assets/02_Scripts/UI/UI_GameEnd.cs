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
        WaveManager.Instance.PlayerEntity.SkillSystem.CancelAll();
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
        waveText.text = $"WAVE : {WaveManager.Instance.CurrentWave - 1}";
    }

    private void ItemSlotSetting()
    {
        foreach (var item in WaveManager.Instance.GainedItems)
        {
            var itemSlot = Instantiate(this.itemSlot, itemSlotParent).GetComponent<ItemSlot>();
            itemSlot.SetItem(item);
        }
    }

    private void OnClickGoMainGame()
    {
        //SoundManager.Instance.Play(clickSound);
        Managers.SoundManager.Clear();
        foreach (var item in WaveManager.Instance.GainedItems)
        {
            Managers.InventoryManager.AddItem(item);
        }

        SceneTransitioner.Instance.StartTransitioning(SceneType.MainScene);
    }
    
    public override void Close()
    {
        panel.SetActive(false);
    }
}
