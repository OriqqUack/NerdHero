using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainScene : MonoSingleton<UI_MainScene>
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private AudioClip onClickSound;

    [SerializeField] private WindowHolder shop;
    [SerializeField] private WindowHolder profile;
    [SerializeField] private WindowHolder quest;
    [SerializeField] private WindowHolder setting;
    [SerializeField] private WindowHolder mailBox;
    [SerializeField] private WindowHolder rewardBox;
    [SerializeField] private WindowHolder energyCharge;
    [SerializeField] private WindowHolder equipment;
    [SerializeField] private WindowHolder equipmentDetailPopup;
    
    
    private void Start()
    {
        ButtonSetting();
    }

    private void ButtonSetting()
    {
        startGameButton.onClick.AddListener(() => OnClickStartGame());
    }
    
    private void OnClickStartGame()
    {
        SoundManager.Instance.Play(onClickSound);
        SoundManager.Instance.Clear();
        GameManager.Instance.WaveData = CircleExpositor.Instance.CurrentIsland.WaveData;
        GameManager.Instance.CurrentIslandIndex = CircleExpositor.Instance.CurrentTargetIndex;
        if (EnergyManager.Instance.UseEnergy(5))
        {
            SceneTransitioner.Instance.StartTransitioning(SceneType.InGameScene, 1, 0);
        }
    }

    

    public void OpenShop() => shop.OpenWindow();
    public void OpenProfile() => profile.OpenWindow();
    public void OpenQuest() => quest.OpenWindow();
    public void OpenRewardBox() => rewardBox.OpenWindow();
    public void OpenSetting() => setting.OpenWindow();
    public void OpenMailBox() => mailBox.OpenWindow();
    public void OpenEnergyCharge() => energyCharge.OpenWindow();
    public void OpenEquipment() => equipment.OpenWindow();
    public void OpenEquipmentDetailPopup(ItemSO item)
    {
        UI_EquipmentDetailPopup window = equipmentDetailPopup.OpenWindow() as UI_EquipmentDetailPopup;
        if (window != null) window.SetupItem(item);
    }
}
