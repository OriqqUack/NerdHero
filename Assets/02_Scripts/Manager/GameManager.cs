using System;
using DG.Tweening;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>, ISaveable
{
    
    #region Event Handler
    public event Action<int> OnScoreChanged;
    public event Action<int> OnGoldChanged;
    public event Action<int> OnGemChanged;
    public event Action<int> OnHighScoreChanged;
    public event Action<int> OnPlayerLevelChanged;
    public event Action<int> OnIslandIndexChanged;
    public event Action<int> OnEnergyChanged;
    public event Action<int> OnExpChanged;
    #endregion
    private Entity _playerEntity;
    private int _score, _gold, _gem, _highScore, _playerLevel, _currentIslandIndex, _currentEnergy, _totalStageClear, _ranking, _totalKills, _victories;
    private string _profileFrameName, _profileIconName;
    private string _playerName, _playerId;
    private Stat _playerExp, _playerHealth, _playerDamage, _dodgePer, _criticalPer, _energyChargeRate; //_playerSkillDamage, _playerDefense;
    private SOWaveData _waveData;
    private bool _isCleared;

    private bool _isLoaded;
    #region Property
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            Managers.BackendManager.UpdateField<int>("score", value);
            OnScoreChanged?.Invoke(value);
        }
    }

    public int Gold
    {
        get => _gold;
        set
        {
            _gold = value;
            Managers.BackendManager.UpdateField<int>("gold", value);
            OnGoldChanged?.Invoke(value);
        }
    }

    public int Gem
    {
        get => _gem;
        set
        {
            _gem = value;
            Managers.BackendManager.UpdateField<int>("gem", value);
            OnGemChanged?.Invoke(value);
        }
    }

    public int HighScore
    {
        get => _highScore;
        set
        {
            _highScore = value;
            Managers.BackendManager.UpdateField<int>("highScore", value);
            OnHighScoreChanged?.Invoke(value);
        }
    }

    public int PlayerLevelRaw
    {
        get => _playerLevel;
        set
        {
            _playerLevel = value;
            Managers.BackendManager.UpdateField<int>("playerLevel", value);
            OnPlayerLevelChanged?.Invoke(value);
        }
    }
    
    public int CurrentIslandIndex
    {
        get => _currentIslandIndex;
        set
        { 
            _currentIslandIndex = value;
            Managers.BackendManager.UpdateField<int>("currentIslandIndex", value);
            OnIslandIndexChanged?.Invoke(value);
        }
    }

    public int CurrentEnergy
    {
        get => _currentEnergy;
        set
        {
            _currentEnergy = value;
            Managers.BackendManager.UpdateField<int>("currentEnergy", value);
            OnEnergyChanged?.Invoke(value);
        }
    }

    public int CurrentExp
    {
        get => (int)_playerExp.Value;
        set
        {
            _playerExp.DefaultValue = value;
            Managers.BackendManager.UpdateField<int>("currentExp", value);
            OnExpChanged?.Invoke(value);
        }
    }

    public int TotalStageClear
    {
        get => _totalStageClear;
        set
        {
            _totalStageClear = value;
            Managers.BackendManager.UpdateField<int>("totalStageClear", value);
        }
    }

    public int Ranking
    {
        get => _ranking;
        set
        {
            _ranking = value;
            Managers.BackendManager.UpdateField<int>("ranking", value);
        }
    }

    public int TotalKills
    {
        get => _totalKills;
        set
        {
            _totalKills = value;
            Managers.BackendManager.UpdateField<int>("totalKills", value);
        }
    }

    public int Victories
    {
        get => _victories;
        set
        {
            _victories = value;
            Managers.BackendManager.UpdateField<int>("victories", value);
        }
    }

    public string ProfileFrameName
    {
        get => _profileFrameName;
        set
        {
            _profileFrameName = value;
            Managers.BackendManager.UpdateField<string>("profileFrameName", value);
        }
    }

    public string ProfileIconName
    {
        get => _profileIconName;
        set
        {
            _profileIconName = value;
            Managers.BackendManager.UpdateField<string>("profileIconName", value);
        }
    }

    public string PlayerName
    {
        get => _playerName;
        set
        {
            _playerName = value;
            Managers.BackendManager.UpdateField<string>("playerName", value);
        }
    }

    public string PlayerId
    {
        get => _playerId;
        set
        {
            _playerId = value;
            Managers.BackendManager.UpdateField<string>("playerId", value);
        }
    }
    public bool IsClear
    {
        get => _isCleared;
        set
        {
            Debug.Log($"value Changed {value.ToString()}");
            _isCleared = value;
            Managers.BackendManager.UpdateField<bool>("isCleared", value);
        }
    }
    #endregion

    public SOWaveData WaveData
    {
        get => _waveData;
        set => _waveData = value;
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        _playerEntity = GetComponent<Entity>();
        _playerExp = _playerEntity.Stats.GetStat("PLAYER_EXP");
        _playerExp.onValueMax += OnExpMaxed;
        _playerHealth = _playerEntity.Stats.GetStat("PLAYER_MAX_HEALTH");
        _playerDamage = _playerEntity.Stats.GetStat("PLAYER_DAMAGE");
        _dodgePer = _playerEntity.Stats.GetStat("DODGE_PERCENT");
        _criticalPer = _playerEntity.Stats.GetStat("CRITICAL_PER");
        _energyChargeRate = _playerEntity.Stats.GetStat("ENERGY_CHARGE_RATE");
    }
    
    public Stat GetPlayerExpStat() => _playerExp;
    public Stat GetPlayerHealthStat() => _playerHealth;
    public Stat GetPlayerDamageStat() => _playerDamage;
    public Stat GetDodgePerStat() => _dodgePer;
    public Stat GetCriticalPerStat() => _criticalPer;
    public Stat GetEnergyAmountStat() => _energyChargeRate;
    public Entity GetPlayerEntity() => _playerEntity;
    public void SetHpToMaxHp() => _playerEntity.Stats.HPStat.DefaultValue = _playerHealth.Value;
    
    private void OnExpMaxed(Stat stat, float currentValue, float prevValue)
    {
        PlayerLevelRaw++;
    }
    
    public void Save(GameData data)
    {
    }

    public void Load(GameData data)
    {
        if (_isLoaded) return;
        _isLoaded = true;
        
        _score = data.score;
        _gold = data.gold;
        _gem = data.gem;
        _highScore = data.highScore;
        _playerLevel = data.playerLevel;
        _currentIslandIndex = data.currentIslandIndex;
        _currentEnergy = data.currentEnergy;
        _totalKills = data.totalKills;
        _victories = data.victories;
        _ranking = data.ranking;
        
        _profileFrameName = data.profileFrameName;
        _profileIconName = data.profileIconName;
        
        _profileFrameName = data.profileFrameName;
        _profileIconName = data.profileIconName;
        
        _playerName = data.playerName;
        _playerId = data.playerId;

        _isCleared = data.isCleared;
    }
}
