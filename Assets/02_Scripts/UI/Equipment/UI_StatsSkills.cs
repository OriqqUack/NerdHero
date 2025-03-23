using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_StatsSkills : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpStatsText;
    [SerializeField] private TextMeshProUGUI damageStatsText;
    [SerializeField] private TextMeshProUGUI skillDamageStatsText;
    [SerializeField] private TextMeshProUGUI defenseStatsText;

    private Coroutine _hpCo;
    private Coroutine _damageCo;
    private Coroutine _skillDamageCo;
    private Coroutine _defenseCo;

    private float _hpCurrentValue;
    private float _damageCurrentValue;
    private float _skillDamageCurrentValue;
    private float _defenseCurrentValue;
    private void Start()
    {
        GameManager.Instance.GetPlayerHealthStat().onValueChanged += UpdateHp;
        GameManager.Instance.GetPlayerDamageStat().onValueChanged += UpdateDamage;
        GameManager.Instance.GetPlayerSkillDamageStat().onValueChanged += UpdateSkillDamage;
        GameManager.Instance.GetPlayerDefenseStat().onValueChanged += UpdateDefense;

        Init();
    }

    private void OnEnable()
    {
        hpStatsText.text = _hpCurrentValue.ToString();
        damageStatsText.text = _damageCurrentValue.ToString();
        skillDamageStatsText.text = _skillDamageCurrentValue.ToString();
        defenseStatsText.text = _defenseCurrentValue.ToString();
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.GetPlayerHealthStat().onValueChanged -= UpdateHp;
            GameManager.Instance.GetPlayerDamageStat().onValueChanged -= UpdateDamage;
            GameManager.Instance.GetPlayerSkillDamageStat().onValueChanged -= UpdateSkillDamage;
            GameManager.Instance.GetPlayerDefenseStat().onValueChanged -= UpdateDefense;
        }
    }

    private void Init()
    {
        UpdateHp(null, GameManager.Instance.GetPlayerHealthStat().Value, 0);
        UpdateDamage(null, GameManager.Instance.GetPlayerDamageStat().Value, 0);
        UpdateSkillDamage(null, GameManager.Instance.GetPlayerSkillDamageStat().Value, 0);
        UpdateDefense(null, GameManager.Instance.GetPlayerDefenseStat().Value, 0);
    }

    private void UpdateHp(Stat stat, float currentHp, float prevHp)
    {
        _hpCurrentValue = currentHp;
        if (_hpCo != null)
            StopCoroutine(_hpCo);  // 기존 코루틴 중지

        _hpCo = StartCoroutine(ChangeNumber(hpStatsText, currentHp, prevHp));
    }
    private void UpdateDamage(Stat stat, float currentDamage, float prevDamage)
    {
        _damageCurrentValue = currentDamage;
        if (_damageCo != null)
            StopCoroutine(_damageCo);  // 기존 코루틴 중지

        _damageCo = StartCoroutine(ChangeNumber(damageStatsText, currentDamage, prevDamage));
    }
    private void UpdateSkillDamage(Stat stat, float currentSkillDamage, float prevSkillDamage)
    {
        _skillDamageCurrentValue = currentSkillDamage;
        
        if (_skillDamageCo != null)
            StopCoroutine(_skillDamageCo);  // 기존 코루틴 중지

        _skillDamageCo = StartCoroutine(ChangeNumber(skillDamageStatsText, currentSkillDamage, prevSkillDamage));
    }
    private void UpdateDefense(Stat stat, float currentDefense, float prevDefense)
    {
        _defenseCurrentValue = currentDefense;
        if (_defenseCo != null)
            StopCoroutine(_defenseCo);  // 기존 코루틴 중지

        _defenseCo = StartCoroutine(ChangeNumber(defenseStatsText, currentDefense, prevDefense));
    }

    // 숫자가 점진적으로 변하는 코루틴
    IEnumerator ChangeNumber(TextMeshProUGUI text, float target, float current)
    {
        float duration = 0.5f; // 카운팅에 걸리는 시간 설정
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            current = Mathf.MoveTowards(current, target, Mathf.Abs(target - current) / duration * Time.deltaTime);
            text.text = ((int)current).ToString();
            yield return null;
        }

        text.text = ((int)target).ToString();
    }
}
