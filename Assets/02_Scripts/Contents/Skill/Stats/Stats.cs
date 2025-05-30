using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Stats : MonoBehaviour
{
    #region Property
    [SerializeField] private Stat maxHpStat;
    [SerializeField] private Stat hpStat;
    [SerializeField] private Stat maxSkillCostStat;
    [SerializeField] private Stat skillCostStat;
    [SerializeField] private Stat damageStat;
    [SerializeField] private Stat damageReductionStat;
    [SerializeField] private Stat criticalDamageStat;
    [SerializeField] private Stat criticalPerStat;
    [SerializeField] private Stat expChargeStat;
    [SerializeField] private Stat heartDropRateStat;

    [Space]
    [SerializeField] private StatGrowthTable statGrowthTable; // 테이블 연결
    [SerializeField] private StatOverride[] statOverrides;

    private Stat[] stats;

    public Entity Owner { get; private set; }
    public int Level = 1;
    public Stat MaxHpStat { get; private set; }
    public Stat HPStat { get; private set; }
    public Stat MaxSkillCostStat { get; private set; }
    public Stat SkillCostStat { get; private set; }
    public Stat Damage { get; private set; }
    public Stat DamageReduction { get; private set; }
    public Stat CriticalDamage { get; private set; }
    public Stat CriticalPer { get; private set; }
    public Stat ExpCharge { get; private set; }
    public Stat HeartDropRate { get; private set; }
    #endregion

    #region GUI
    /*private void OnGUI()
    {
        if (!Owner.IsPlayer)
            return;

        // ���� ��ܿ� ���� Box�� �׷���
        GUI.Box(new Rect(2f, 2f, 250f, 250f), string.Empty);

        // �ڽ� �� �κп� Player Stat Text�� �߿���
        GUI.Label(new Rect(4f, 2f, 100f, 30f), "Player Stat");

        var textRect = new Rect(4f, 22f, 200f, 30f);
        // Stat ������ ���� + Button�� ���� ��ġ
        var plusButtonRect = new Rect(textRect.x + textRect.width, textRect.y, 20f, 20f);
        // Stat ���Ҹ� ���� - Button�� ���� ��ġ
        var minusButtonRect = plusButtonRect;
        minusButtonRect.x += 22f;

        foreach (var stat in stats)
        {
            // % Type�̸� ���ϱ� 100�� �ؼ� 0~100���� ���
            // 0.##;-0.## format�� �Ҽ��� 2��°¥������ ����ϵ�
            // ����� �״�� ���, ������ -�� �ٿ��� ����϶�� ��
            string defaultValueAsString = stat.IsPercentType ?
                $"{stat.DefaultValue * 100f:0.##;-0.##}%" :
                stat.DefaultValue.ToString("0.##;-0.##");

            string bonusValueAsString = stat.IsPercentType ?
                $"{stat.BonusValue * 100f:0.##;-0.##}%" :
                stat.BonusValue.ToString("0.##;-0.##");

            GUI.Label(textRect, $"{stat.DisplayName}: {defaultValueAsString} ({bonusValueAsString})");
            // + Button�� ������ Stat ����
            if (GUI.Button(plusButtonRect, "+"))
            {
                if (stat.IsPercentType)
                    stat.DefaultValue += 0.01f;
                else
                    stat.DefaultValue += 1f;
            }

            // - Button�� ������ Stat ����
            if (GUI.Button(minusButtonRect, "-"))
            {
                if (stat.IsPercentType)
                    stat.DefaultValue -= 0.01f;
                else
                    stat.DefaultValue -= 1f;
            }

            // ���� Stat ���� ����� ���� y������ ��ĭ ����
            textRect.y += 22f;
            plusButtonRect.y = minusButtonRect.y = textRect.y;
        }
    }*/
    #endregion

    #region Stat Methods
    public void Setup(Entity entity, int level = 1)
    {
        Owner = entity;

        stats = statOverrides.Select(x => x.CreateStat()).ToArray();
        
        MaxHpStat = maxHpStat ? GetStat(maxHpStat) : null;
        HPStat = hpStat ? GetStat(hpStat) : null;
        
        MaxSkillCostStat = skillCostStat ? GetStat(maxSkillCostStat) : null;
        SkillCostStat = skillCostStat ? GetStat(skillCostStat) : null;

        ClearedStats();
        
        Damage = damageStat ? GetStat(damageStat) : null;
        DamageReduction = damageReductionStat ? GetStat(damageReductionStat) : null;
        CriticalDamage = criticalDamageStat ? GetStat(criticalDamageStat) : null;
        CriticalPer = criticalPerStat ? GetStat(criticalPerStat) : null;
        ExpCharge = expChargeStat ? GetStat(expChargeStat) : null;
        HeartDropRate = heartDropRateStat ? GetStat(heartDropRateStat) : null;
        
        //사거리 조절
        if (entity.BaseAttack)
        {
            var stat = stats.First(x => x.CodeName == "ATTACK_RANGE");
            stat.onValueChanged += (stat1, value, prevValue) =>
            {
                BoxCollider collider = entity.BaseAttack.GetComponent<BoxCollider>();
                Vector3 originalSize = collider.size;
                Vector3 originalCenter = collider.center;

                float newLength = originalSize.z + value;
                collider.size = new Vector3(originalSize.x, originalSize.y, newLength);
                collider.center = new Vector3(originalCenter.x, originalCenter.y, originalCenter.z + value / 2f);
            };
        }
    }

    public void ClearedStats()
    {
        if(hpStat)
        {
            MaxHpStat.onValueChanged -= OnChangedHpMaxValue;
            MaxHpStat.onValueChanged += OnChangedHpMaxValue;
            HPStat.MaxValue = MaxHpStat.Value;
            HPStat.DefaultValue = HPStat.MaxValue;
        }
        
        if(SkillCostStat)
        {
            MaxSkillCostStat.onValueChanged -= OnChangedSkillCostMaxValue;
            MaxSkillCostStat.onValueChanged += OnChangedSkillCostMaxValue;
            SkillCostStat.MaxValue = MaxSkillCostStat.Value;
        }
    }
    
    public void LevelSetup(int level)
    {
        UpdateStatsByLevel(null, level, 0);
    }

    private void OnChangedHpMaxValue(Stat stat, float currentValue, float prevValue)
    {
        HPStat.MaxValue = MaxHpStat.Value;
    }
    
    private void OnChangedSkillCostMaxValue(Stat stat, float currentValue, float prevValue)
    {
        SkillCostStat.MaxValue = MaxSkillCostStat.Value;
    }

    public void SetupOwner(Entity entity)
    {
        Owner = entity;
        Setup(entity);
    }
    
    private void OnDestroy()
    {
        foreach (var stat in stats)
            Destroy(stat);
        stats = null;
    }
    
    public Stat GetStat(Stat stat)
    {
        Debug.Assert(stat != null, $"Stats::GetStat - stat�� null�� �� �� �����ϴ�.");
        return stats.FirstOrDefault(x => x.ID == stat.ID);
    }

    public Stat GetStat(string statName)
    {
        Stat stat = stats.FirstOrDefault(x => x.CodeName == statName);
        Debug.Assert(stat != null, $"Stats::GetStat - stat�� null�� �� �� �����ϴ�.");
        return stat;
    }

    public bool TryGetStat(Stat stat, out Stat outStat)
    {
        Debug.Assert(stat != null, $"Stats::TryGetStat - stat�� null�� �� �� �����ϴ�.");

        outStat = stats.FirstOrDefault(x => x.ID == stat.ID);
        return outStat != null;
    }

    public float GetValue(Stat stat)
        => GetStat(stat).Value;

    public bool HasStat(Stat stat)
    {
        Debug.Assert(stat != null, $"Stats::HasStat - stat�� null�� �� �� �����ϴ�.");
        return stats.Any(x => x.ID == stat.ID);
    }

    #endregion

    #region Public Methods
    private void UpdateStatsByLevel(Stat stat, float currentValue, float prevValue)
    {
        if (!statGrowthTable) return;
        var growthData = statGrowthTable.GrowthDataList.Find(x => x.Level == (int)currentValue);
        if (growthData == null)
        {
            Debug.LogWarning($"레벨 {currentValue}에 해당하는 스탯 데이터가 없습니다!");
            return;
        }
        
        SetDefaultValue(MaxHpStat, growthData.MaxHp);
        HPStat.MaxValue = MaxHpStat.Value;
        HPStat.DefaultValue = HPStat.MaxValue;
        
        SetDefaultValue(Damage, growthData.Damage);
        SetDefaultValue(ExpCharge, growthData.expChargeAmount);
        SetDefaultValue(HeartDropRate, growthData.heartDropRate);
    }
    
    public void SetDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue = value;

    public float GetDefaultValue(Stat stat)
        => GetStat(stat).DefaultValue;

    public void IncreaseDefaultValue(Stat stat, float value)
        => GetStat(stat).DefaultValue += value;

    public void InCreaseDeafaultVaue(string statName, float value)
    {
        Stat stat = stats.FirstOrDefault(x => x.CodeName == statName);
        Debug.Assert(stat != null, $"Stats::GetStat - stat�� null�� �� �� �����ϴ�.");
        IncreaseDefaultValue(stat, value);
    }
    public void SetBonusValue(Stat stat, object key, float value)
        => GetStat(stat).SetBonusValue(key, value);
    public void SetBonusValue(Stat stat, object key, object subKey, float value)
        => GetStat(stat).SetBonusValue(key, subKey, value);

    public float GetBonusValue(Stat stat)
        => GetStat(stat).BonusValue;
    public float GetBonusValue(Stat stat, object key)
        => GetStat(stat).GetBonusValue(key);
    public float GetBonusValue(Stat stat, object key, object subKey)
        => GetStat(stat).GetBonusValue(key, subKey);
    
    public void RemoveBonusValue(Stat stat, object key)
        => GetStat(stat).RemoveBonusValue(key);
    public void RemoveBonusValue(Stat stat, object key, object subKey)
        => GetStat(stat).RemoveBonusValue(key, subKey);

    public bool ContainsBonusValue(Stat stat, object key)
        => GetStat(stat).ContainsBonusValue(key);
    public bool ContainsBonusValue(Stat stat, object key, object subKey)
        => GetStat(stat).ContainsBonusValue(key, subKey);
    #endregion

    #region Editor Menu
#if UNITY_EDITOR
    [ContextMenu("LoadStats")]
    private void LoadStats()
    {
        var stats = Resources.LoadAll<Stat>("Stat").OrderBy(x => x.ID);
        statOverrides = stats.Select(x => new StatOverride(x)).ToArray();
    }
#endif
    #endregion
}
