using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainScene : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingButton;

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioClip onClickSound;
    private void Start()
    {
        ButtonSetting();
    }

    private void ButtonSetting()
    {
        startGameButton.onClick.AddListener(() => OnClickStartGame());
        settingButton.onClick.AddListener(() => OnClickSetting());
    }
    
    private void OnClickStartGame()
    {
        SoundManager.Instance.Play(onClickSound);
        SoundManager.Instance.Clear();
        SceneTransitionManager.LoadSceneInstantly("Scene_InGame");
    }

    private void OnClickSetting()
    {
        SoundManager.Instance.Play(onClickSound);
        settingsPanel.SetActive(true);
    }
}
