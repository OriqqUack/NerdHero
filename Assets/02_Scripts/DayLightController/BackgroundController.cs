using System;
using DG.Tweening;
using FunkyCode;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private CanvasGroup nightCanvasGroup;
    [SerializeField] private LightCycle lightCycle;
    [SerializeField] private AudioClip dayLightSound;
    [SerializeField] private AudioClip nightLightSound;

    private int waveCount;
    private float _currentNightCanvasAlpha = 0;
    private float _offset;
    private void Start()
    {
        SoundManager.Instance.Play(dayLightSound, Sound.Bgm);
        WaveManager.Instance.OnWaveChange += ChangeWave;
        waveCount = WaveManager.Instance.TotalWaveCount;
        _offset = 1.0f/waveCount;
    }

    private void ChangeWave(int wave)
    {
        _currentNightCanvasAlpha += _offset;
        nightCanvasGroup.DOFade(_currentNightCanvasAlpha, 2f);
        DOTween.To(() => lightCycle.time, x => lightCycle.time = x, _currentNightCanvasAlpha, 2f);

        if (wave >= waveCount - 2)
        {
            SoundManager.Instance.FadeOutBgm(1f, () =>
            {
                SoundManager.Instance.FadeInBgm(nightLightSound, 1f, 1f);
            });
        }
    }
}
