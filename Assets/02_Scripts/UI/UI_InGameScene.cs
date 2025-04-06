using System;
using UnityEngine;

public class UI_InGameScene : MonoSingleton<UI_InGameScene>
{
    [SerializeField] private WindowHolder pause;
    [SerializeField] private WindowHolder exitAlert;
    [SerializeField] private WindowHolder revive;
    [SerializeField] private WindowHolder gameEnd;

    private void Start()
    {
        WaveManager.Instance.OnWaveEnd += OpenGameEnd;
    }

    public void OpenPause() => pause.OpenWindow();
    public void OpenExitAlert() => exitAlert.OpenWindow();
    public void OpenRevive()
    {
        Time.timeScale = 0;
        revive.OpenWindow();
    }

    public void OpenGameEnd()
    {
        Time.timeScale = 0;
        gameEnd.OpenWindow();
    }
}
