using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_MainScene : MonoSingleton<UI_MainScene>
{
    [SerializeField] private float uiDuration = 0.5f;
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
    [SerializeField] private WindowHolder nickNameChanger;

    private UiWindow _currentWindow;
    private WindowHolder _currentHolder;
    
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
        Managers.SoundManager.Play(onClickSound);
        Managers.SoundManager.Clear();
        GameManager.Instance.WaveData = CircleExpositor.Instance.CurrentIsland.WaveData;
        GameManager.Instance.CurrentIslandIndex = CircleExpositor.Instance.CurrentTargetIndex;
        if (Managers.EnergyManager.UseEnergy(5))
        {
            SceneTransitioner.Instance.StartTransitioning(SceneType.InGameScene, 1, 0);
        }
    }

    public void OpenShop() => OpenNewWindow(shop);
    public void OpenProfile() => profile.OpenWindow();
    public void OpenQuest() => quest.OpenWindow();
    public void OpenRewardBox() => rewardBox.OpenWindow();
    public void OpenSetting() => setting.OpenWindow();
    public void OpenMailBox() => mailBox.OpenWindow();
    public void OpenEnergyCharge() => energyCharge.OpenWindow();
    public void OpenEquipment() => OpenNewWindow(equipment);
    public void OpenEquipmentDetailPopup(ItemSO item)
    {
        UI_EquipmentDetailPopup window = equipmentDetailPopup.OpenWindow() as UI_EquipmentDetailPopup;
        if (window != null) window.SetupItem(item);
    }
    
    public void OpenNickNameChanger() => nickNameChanger.OpenWindow();

    public void CloseCurrentWindow()
    {
        if(_currentWindow)
            SlideHide(_currentWindow);
    }
    
    private void OpenNewWindow(WindowHolder newHolder)
    {
        if (_currentHolder == newHolder) return;
        
        if (_currentWindow != null)
        {
            // 현재 열려 있는 창 닫기
            SlideHide(_currentWindow);
        }
        
        // 새 창 열기
        _currentWindow = newHolder.OpenWindow();
        _currentHolder = newHolder;

        if (_currentWindow != null)
        {
            SlideShow(_currentWindow);
        }
    }

// 🔹 슬라이드로 보여주기 (아래 -> 중앙 + 알파 0->1)
    private void SlideShow(UiWindow window)
    {
        RectTransform panel = window.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

        if (panel != null && canvasGroup != null)
        {
            Vector2 centerPos = Vector2.zero;
            Vector2 hiddenPos = new Vector2(0, -Screen.height);
            float duration = uiDuration; // 

            // 시작 위치와 알파 설정
            panel.anchoredPosition = hiddenPos;
            canvasGroup.alpha = 0f;

            // 애니메이션 실행
            Sequence seq = DOTween.Sequence();
            seq.Append(panel.DOAnchorPos(centerPos, duration).SetEase(Ease.OutCubic));
            seq.Join(canvasGroup.DOFade(1f, duration));
        }
    }

// 🔹 슬라이드로 숨기기 (중앙 -> 아래 + 알파 1->0)
    private void SlideHide(UiWindow window)
    {
        RectTransform panel = window.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = window.GetComponent<CanvasGroup>();

        if (panel != null && canvasGroup != null)
        {
            Vector2 hiddenPos = new Vector2(0, -Screen.height);
            float duration = uiDuration;

            // 애니메이션 실행 후 완전히 닫기
            Sequence seq = DOTween.Sequence();
            seq.Append(panel.DOAnchorPos(hiddenPos, duration).SetEase(Ease.InCubic));
            seq.Join(canvasGroup.DOFade(0f, duration));
        }
    }
}
