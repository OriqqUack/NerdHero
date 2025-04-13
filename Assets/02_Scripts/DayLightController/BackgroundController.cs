using System;
using DG.Tweening;
using FunkyCode;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private CanvasGroup dayCanvasGroup;
    [SerializeField] private CanvasGroup nightCanvasGroup;
    [SerializeField] private LightCycle lightCycle;

    private int waveCount;
    private float _currentDayCanvasAlpha = 1;
    private float _currentNightCanvasAlpha = 0;
    private float _offset;
    private void Start()
    {
        WaveManager.Instance.OnWaveChange += ChangeWave;
        waveCount = WaveManager.Instance.TotalWaveCount;
        _offset = 1.0f/waveCount;
    }

    private void ChangeWave(int wave)
    {
        _currentDayCanvasAlpha -= _offset;
        _currentNightCanvasAlpha += _offset;
        dayCanvasGroup.DOFade(_currentDayCanvasAlpha, 2f);
        nightCanvasGroup.DOFade(_currentNightCanvasAlpha, 2f);
        DOTween.To(() => lightCycle.time, x => lightCycle.time = x, _currentNightCanvasAlpha, 2f);
    }
}
