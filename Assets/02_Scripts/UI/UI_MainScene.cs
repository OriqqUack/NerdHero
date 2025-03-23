using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainScene : MonoBehaviour
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
        SceneTransitionManager.LoadSceneInstantly("Scene_InGame");
    }

    public void OpenShop() => shop.OpenWindow();
    public void OpenProfile() => profile.OpenWindow();
    public void OpenQuest() => quest.OpenWindow();
    public void OpenRewardBox() => rewardBox.OpenWindow();
    public void OpenSetting() => setting.OpenWindow();
    public void OpenMailBox() => mailBox.OpenWindow();
    public void OpenEnergyCharge() => energyCharge.OpenWindow();
    
}
