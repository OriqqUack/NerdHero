using System;
using DG.Tweening;
using UnityEngine;

public class UI_InGameScene : MonoSingleton<UI_InGameScene>
{
    [SerializeField] private WindowHolder pause;
    [SerializeField] private WindowHolder exitAlert;
    [SerializeField] private WindowHolder revive;
    [SerializeField] private WindowHolder gameEnd;
    [SerializeField] private WindowHolder cardSelec;

    private UiWindow _cardSelectUI;
    private bool _isGameEnd;
    private void Start()
    {
        DOTween.defaultTimeScaleIndependent = true;
        WaveManager.Instance.OnWaveEnd += OpenGameEnd;
        WaveManager.Instance.PlayerEntity.Stats.GetStat("LEVEL").onValueChanged += OpenCardSelec;
    }
    
    public UiWindow GetCardSelectUI() => _cardSelectUI;

    public void OpenPause() => pause.OpenWindow();
    public void OpenExitAlert() => exitAlert.OpenWindow();
    public void OpenRevive()
    {
        Time.timeScale = 0;
        revive.OpenWindow();
    }

    public void OpenGameEnd()
    {
        _isGameEnd = true;
        Time.timeScale = 0;
        gameEnd.OpenWindow();
    }

    private void OpenCardSelec(Stat stat, float currentValue, float prevValue)
    {
        if (_isGameEnd) return;
        Time.timeScale = 0;
        if(_cardSelectUI)
        {
            _cardSelectUI.gameObject.SetActive(true);
            (_cardSelectUI as CardAnimator).SpawnCards((int)currentValue);
        }
        else
        {
            _cardSelectUI = cardSelec.OpenWindow();
        }
    }
}
