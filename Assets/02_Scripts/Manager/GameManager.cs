using System;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>, ISaveable
{
    private Entity _playerEntity;
    private Stat _playerExp, _playerLevel, _playerGold, _playerHealth, _playerDamage; //_playerSkillDamage, _playerDefense;
    private Skill _playerSkill1, _playerSkill2;

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
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(this);
        _playerEntity = GetComponent<Entity>();
        _playerExp = _playerEntity.Stats.GetStat("PLAYER_EXP");
        _playerLevel = _playerEntity.Stats.GetStat("PLAYER_LEVEL");
        _playerGold = _playerEntity.Stats.GetStat("PLAYER_GOLD");
        _playerHealth = _playerEntity.Stats.GetStat("PLAYER_HEALTH");
        _playerDamage = _playerEntity.Stats.GetStat("PLAYER_DAMAGE");
        /*_playerSkillDamage = _playerEntity.Stats.GetStat("PLAYER_SKILL_DAMAGE");
        _playerDefense = _playerEntity.Stats.GetStat("PLAYER_DEFENSE");*/
    }
    
    public Stat GetPlayerExpStat() => _playerExp;
    public Stat GetPlayerLevelStat() => _playerLevel;
    public Stat GetPlayerGoldStat() => _playerGold;
    public Stat GetPlayerHealthStat() => _playerHealth;
    public Stat GetPlayerDamageStat() => _playerDamage;
    /*public Stat GetPlayerSkillDamageStat() => _playerSkillDamage;
    public Stat GetPlayerDefenseStat() => _playerDefense;*/
    public Entity GetPlayerEntity() => _playerEntity;
    
    public void Save(SaveData data)
    {
    }

    public void Load(SaveData data)
    {
    }
}
