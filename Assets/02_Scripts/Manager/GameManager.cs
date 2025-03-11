using System;
using UnityEngine;

public class GameManager : MonoSingleton<GameManager>
{
    private Entity _playerEntity;
    private Stat _playerExp;
    private Stat _playerLevel;
    private Stat _playerGold;
    
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
    
    private void Awake()
    {
        DontDestroyOnLoad(this);
        _playerEntity = GetComponent<Entity>();
        _playerExp = _playerEntity.Stats.GetStat("PLAYER_EXP");
        _playerLevel = _playerEntity.Stats.GetStat("PLAYER_LEVEL");
        _playerGold = _playerEntity.Stats.GetStat("PLAYER_GOLD");
    }
    
    public Stat GetPlayerExpStat() => _playerExp;
    public Stat GetPlayerLevelStat() => _playerLevel;
    public Stat GetPlayerGoldStat() => _playerGold;
}
