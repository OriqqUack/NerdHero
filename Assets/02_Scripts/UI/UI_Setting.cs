using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Setting : UI_Popup
{
    [SerializeField] private GameObject settingPanel;
    [Header("User Info")] 
    [SerializeField] private TextMeshProUGUI userID;
    
    [Space(10)]
    [Header("Sound Settings")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectsSlider;
    [Range(0f, 1f)] [SerializeField] private float masterVolume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float bgmVolume = 0.5f;
    [Range(0f, 1f)] [SerializeField] private float effectsVolume = 0.5f;
    
    [Space(10)]
    [Header("Language Settings")]
    [SerializeField] private Button koreanBtn;
    [SerializeField] private Button englishBtn;

    private void Start()
    {
        AudioInit();
        settingPanel.SetActive(false);
    }
    
    #region Audio Setting Function

    private void AudioInit()
    {
        masterSlider.value = masterVolume;
        musicSlider.value = bgmVolume;
        effectsSlider.value = effectsVolume;

        ApplyVolumeSettings();
        
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        effectsSlider.onValueChanged.AddListener(SetEffectsVolume);
    }

    private void ApplyVolumeSettings()
    {
        SoundManager.Instance.BgmSource.volume = bgmVolume * masterVolume;
        SoundManager.Instance.EffectSource.volume = effectsVolume * masterVolume;
    }

    private void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
    }

    private void SetMusicVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
    }

    private void SetEffectsVolume(float volume)
    {
        effectsVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
    }

    #endregion
    
    public override void Close()
    {
        SoundManager.Instance.Play(closeSound);
        settingPanel.SetActive(false);
    }
}
