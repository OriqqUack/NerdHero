using System;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>, ISaveable
{
    private Entity _playerEntity;

    private Stat
        _playerExp, _playerLevel, _playerGold, _playerHealth, _playerDamage, _dodgePer, _criticalPer, _energyAmount; //_playerSkillDamage, _playerDefense;

    private Skill _playerSkill1, _playerSkill2;
    private SOWaveData _waveData;
    private int _currentIslandIndex;
    private PlayerProfile _playerProfile;

    public bool IsClear { get; set; }

    public int CurrentIslandIndex
    {
        get => _currentIslandIndex;
        set => _currentIslandIndex = value;
    }

    public SOWaveData WaveData
    {
        get => _waveData;
        set => _waveData = value;
    }

    public Skill PlayerSkill1
    {
        get => _playerSkill1;
        set => _playerSkill1 = value;
    }

    public Skill PlayerSkill2
    {
        get => _playerSkill2;
        set => _playerSkill2 = value;
    }

    public float PlayerExp
    {
        get => _playerExp.Value;
        set => _playerExp.DefaultValue += value;
    }

    public float PlayerLevel
    {
        get => _playerLevel.Value;
        set => _playerLevel.DefaultValue += value;
    }

    public int PlayerGold
    {
        get => (int)_playerGold.Value;
        set => _playerGold.DefaultValue += value;
    }

    public int PlayerHealth
    {
        get => (int)_playerHealth.Value;
        set => _playerHealth.DefaultValue += value;
    }

    public int PlayerDamage
    {
        get => (int)_playerDamage.Value;
        set => _playerDamage.DefaultValue += value;
    }

    public PlayerProfile PlayerProfile => _playerProfile;

    
    /*public int PlayerSkillDamage
    {
        get => (int)_playerSkillDamage.Value;
        set => _playerSkillDamage.DefaultValue += value;
    }
    
    public int PlayerDefense
    {
        get => (int)_playerDefense.Value;
        set => _playerDefense.DefaultValue += value;
    }*/


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 60;
        _playerEntity = GetComponent<Entity>();
        _playerExp = _playerEntity.Stats.GetStat("PLAYER_EXP");
        _playerLevel = _playerEntity.Stats.GetStat("PLAYER_LEVEL");
        _playerGold = _playerEntity.Stats.GetStat("PLAYER_GOLD");
        _playerHealth = _playerEntity.Stats.GetStat("PLAYER_MAX_HEALTH");
        _playerDamage = _playerEntity.Stats.GetStat("PLAYER_DAMAGE");
        _dodgePer = _playerEntity.Stats.GetStat("DODGE_PERCENT");
        _criticalPer = _playerEntity.Stats.GetStat("CRITICAL_PER");
        _energyAmount = _playerEntity.Stats.GetStat("ENERGY_CHARGE_RATE");
        /*_playerSkillDamage = _playerEntity.Stats.GetStat("PLAYER_SKILL_DAMAGE");
        _playerDefense = _playerEntity.Stats.GetStat("PLAYER_DEFENSE");*/
    }
    
    public Stat GetPlayerExpStat() => _playerExp;
    public Stat GetPlayerLevelStat() => _playerLevel;
    public Stat GetPlayerGoldStat() => _playerGold;
    public Stat GetPlayerHealthStat() => _playerHealth;
    public Stat GetPlayerDamageStat() => _playerDamage;
    public Stat GetDodgePerStat() => _dodgePer;
    public Stat GetCriticalPerStat() => _criticalPer;
    public Stat GetEnergyAmountStat() => _energyAmount;
    public Entity GetPlayerEntity() => _playerEntity;
    
    public void Save(SaveData data)
    {
        data.PlayerProfile = _playerProfile;
    }

    public void Load(SaveData data)
    {
        _playerProfile = (PlayerProfile)data.PlayerProfile;
    }
}
