using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_StatsSkills : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpStatsText;
    [SerializeField] private TextMeshProUGUI criticalStatsText;

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
        GameManager.Instance.GetCriticalPerStat().onValueChanged += UpdateCritical;

        Init();
    }

    private void OnEnable()
    {
        hpStatsText.text = _hpCurrentValue.ToString();
        criticalStatsText.text = _damageCurrentValue.ToString();
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.GetPlayerHealthStat().onValueChanged -= UpdateHp;
            GameManager.Instance.GetCriticalPerStat().onValueChanged -= UpdateCritical;
        }
    }

    private void Init()
    {
        UpdateHp(GameManager.Instance.GetPlayerHealthStat(), GameManager.Instance.GetPlayerHealthStat().Value, 0);
        UpdateCritical(GameManager.Instance.GetCriticalPerStat(), GameManager.Instance.GetCriticalPerStat().Value, 0);
    }

    private void UpdateHp(Stat stat, float currentHp, float prevHp)
    {
        _hpCurrentValue = currentHp;
        if (_hpCo != null)
            StopCoroutine(_hpCo);  // 기존 코루틴 중지
        
        bool isPercent = stat.IsPercentType;

        _hpCo = StartCoroutine(ChangeNumber(hpStatsText, currentHp, prevHp, isPercent));
    }
    private void UpdateCritical(Stat stat, float currentDamage, float prevDamage)
    {
        _damageCurrentValue = currentDamage;
        if (_damageCo != null)
            StopCoroutine(_damageCo);  // 기존 코루틴 중지

        bool isPercent = stat.IsPercentType;

        _damageCo = StartCoroutine(ChangeNumber(criticalStatsText, currentDamage, prevDamage, isPercent));
    }

    // 숫자가 점진적으로 변하는 코루틴
    IEnumerator ChangeNumber(TextMeshProUGUI text, float target, float current, bool isPercent)
    {
        float duration = 0.5f; // 카운팅에 걸리는 시간 설정
        float elapsed = 0f;

        if (isPercent)
        {
            current *= 100f;
            current = (int)current;
            target *= 100f;
            target = (int)target;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                current = Mathf.MoveTowards(current, target, Mathf.Abs(target - current) / duration * Time.deltaTime);
                text.text = $"{(int)current} %" ;
                yield return null;
            }
            text.text = $"{((int)(target)).ToString()} %" ;
        }
        else
        {
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                current = Mathf.MoveTowards(current, target, Mathf.Abs(target - current) / duration * Time.deltaTime);
                text.text = ((int)current).ToString() ;
                yield return null;
            }
            text.text = ((int)target).ToString();
        }
    }
}
