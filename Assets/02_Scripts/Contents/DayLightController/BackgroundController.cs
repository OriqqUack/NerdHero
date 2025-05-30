using System;
using DG.Tweening;
using FunkyCode;
using UnityEngine;
using UnityEngine.UI;

//Wave가 진행 될 수록 어두워지는 연출 스크립트
public class BackgroundController : MonoBehaviour
{
    [SerializeField] private CanvasGroup nightCanvasGroup;
    [SerializeField] private LightCycle lightCycle;
    [SerializeField] private AudioClip dayLightSound;
    [SerializeField] private AudioClip nightLightSound;
    [SerializeField] private GameObject[] lights;
    
    private int _waveCount;
    private float _currentNightCanvasAlpha = 0;
    private float _offset;
    private void Start()
    {
        Managers.SoundManager.Play(dayLightSound, Sound.Bgm);
        WaveManager.Instance.OnWaveStart += ChangeWave;
        _waveCount = WaveManager.Instance.TotalWaveCount;
        _offset = 1.0f/_waveCount;
    }

    private void ChangeWave(int wave)
    {
        _currentNightCanvasAlpha += _offset;
        nightCanvasGroup.DOFade(_currentNightCanvasAlpha, 2f);
        DOTween.To(() => lightCycle.time, x => lightCycle.time = x, _currentNightCanvasAlpha, 2f);

        if (wave == _waveCount - 1)
        {
            Managers.SoundManager.FadeOutBgm(1f, () =>
            {
                Managers.SoundManager.FadeInBgm(nightLightSound, 1f, 1f);
            });

            foreach (var light in lights)
            {
                light.SetActive(true);
            }
        }
    }
}
