using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityHUD : MonoSingleton<EntityHUD>
{
    [SerializeField] private TextMeshProUGUI nameText; //

    [Header("Stat View")] [SerializeField] private Slider hpFillImage;
    [SerializeField] private Slider hpFollowImage;
    [SerializeField] private TextMeshProUGUI hpValueText; //
    [SerializeField] private RectTransform panel;

    [Header("Effecf List View")] 
    [SerializeField] private SkillEffectListView effectListView; //
    
    [Header("Effecf List View")]
    public Image sameAxisImage; //
    
    private Entity target;
    private float hpMaxValue;

    protected override void Awake()
    {
        base.Awake();
        panel.gameObject.SetActive(false);
    }

    private void OnDestroy() => ReleaseEvents();

    public void Show(Entity target)
    {
        ReleaseEvents();
        sameAxisImage.gameObject.SetActive(false);

        this.target = target;
        target.onDead += OnEntityDead;

        if (nameText)
            nameText.text = target.name;

        var stats = target.Stats;
        stats.HPStat.onValueChanged += OnHPStatChanged;

        hpMaxValue = stats.HPStat.Value;

        UpdateStatView(stats.HPStat, hpFillImage, hpFollowImage, hpValueText);

        if (effectListView)
            effectListView.Target = target.SkillSystem;
        
        panel.gameObject.SetActive(true);
    }

    public void AxisImageControl(bool value)
    {
        sameAxisImage.gameObject.SetActive(value);
    }

    public void Hide()
    {
        ReleaseEvents();

        target = null;
        if (effectListView)
            effectListView.Target = null;
        
        panel.gameObject.SetActive(false);
    }

    private void UpdateStatView(Stat stat, Slider statFillAmount, Slider hpFollowFillAmount, TextMeshProUGUI statText)
    {
        statFillAmount.value = stat.Value / hpMaxValue;
        hpFollowFillAmount.value = stat.Value / hpMaxValue;
        //statText.text = $"{Mathf.RoundToInt(stat.Value)} / {stat.MaxValue}";
    }
    
    private void UpdateStatViewLerp(Stat stat, Slider statFillAmount, Slider hpFollowFillAmount, TextMeshProUGUI statText)
    {
        StartCoroutine(HpFollow(stat, statFillAmount, hpFollowFillAmount));
        //statText.text = $"{Mathf.RoundToInt(stat.Value)} / {stat.MaxValue}";
    }

    private IEnumerator HpFollow(Stat stat, Slider statFillAmount, Slider hpFollowFillAmount)
    {
        float value = stat.Value / hpMaxValue;
        float speed = 5f; 

        while (Mathf.Abs(statFillAmount.value - value) > 0.01f)
        {
            statFillAmount.value = Mathf.Lerp(statFillAmount.value, value, Time.deltaTime * speed);
            yield return null;
        }
        statFillAmount.value = value;

        yield return new WaitForSeconds(0.2f);

        while (Mathf.Abs(hpFollowFillAmount.value - value) > 0.01f)
        {
            hpFollowFillAmount.value = Mathf.Lerp(hpFollowFillAmount.value, value, Time.deltaTime * speed);
            yield return null;
        }
        hpFollowFillAmount.value = value; 
    }

    private void ReleaseEvents()
    {
        if (!target)
            return;

        target.onDead -= OnEntityDead;
        target.Stats.HPStat.onValueChanged -= OnHPStatChanged;
    }

    private void OnHPStatChanged(Stat stat, float currentValue, float prevValue)
        => UpdateStatViewLerp(stat, hpFillImage, hpFollowImage, hpValueText);

    private void OnEntityDead(Entity entity) => Hide();
}
