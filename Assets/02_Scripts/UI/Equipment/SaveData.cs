using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine;

[System.Serializable]
public class PlayerProfile
{
    public string playerName;          // 플레이어 이름
    public string playerID;
    public int playerLevel;
    public int playerExp;
    public int maxScore;
    public int currentArtifact;
    public int totalStageClears;        // 클리어한 스테이지 수
    public int ranking;                 // 현재 랭킹
    public int totalKills;              // 처치한 몬스터 총 수
    public int victories;               // 승리 횟수
    public string profileFrameName;
    public string profileIconName;
    
    public DateTime lastPlayedDate;     // 마지막으로 게임한 시간
    
    // 생성자
    public PlayerProfile(string name)
    {
        playerName = name;
        playerID = "";
        playerLevel = 1;
        playerExp = 0;
        maxScore = 0;
        totalStageClears = 0;
        ranking = 0;
        totalKills = 0;
        victories = 0;
        lastPlayedDate = DateTime.Now;
        profileFrameName = "Basic";
        profileIconName = "Basic";
    }
    
    public void ClearStage()
    {
        totalStageClears++;
        lastPlayedDate = DateTime.Now;
    }

    // 예시: 몬스터 처치할 때 호출
    public void KillMonster(int killCount = 1)
    {
        totalKills += killCount;
        lastPlayedDate = DateTime.Now;
    }

    // 예시: 승리할 때 호출
    public void WinBattle()
    {
        victories++;
        lastPlayedDate = DateTime.Now;
    }

    // 예시: 랭킹 갱신할 때
    public void UpdateRanking(int newRanking)
    {
        ranking = newRanking;
        lastPlayedDate = DateTime.Now;
    }
}


[System.Serializable] 
public class MapData
{
    public List<bool> clearedMap = new List<bool>();
}

[System.Serializable] 
public class CurrencyData
{
    public int gold;
    public int gem;
    public int energyCount;
    public string energyLastCharge;

    public CurrencyData()
    {
        gold = 0;
        gem = 0;
        energyCount = 100;
        energyLastCharge = "";
    }
}

[System.Serializable] 
public class InventoryData
{
    public List<string> inventoryItems = new List<string>(); // 인벤토리 아이템 (이름 저장)
    public List<int> quantityOrLevel = new List<int>();
    public List<string> skills = new List<string>();
    public List<int> skillLevels = new List<int>();
    public string equippedWeapon;
    public int weaponLevel;
    public string equippedHelmet;
    public int helmetLevel;
    public string equippedArmor;
    public int armorLevel;
    public string equippedBoots;
    public int bootsLevel;
    public string equippedSkill1;
    public string equippedSkill2;
}

[System.Serializable]
public class SaveData
{
    public CurrencyData CurrencyData;
    public PlayerProfile PlayerProfile;
    public InventoryData Inventory;
    public MapData Map;
    
    public SaveData()
    {
        CurrencyData = new CurrencyData();
        PlayerProfile = new PlayerProfile("Douri");
        Inventory = new InventoryData();
        Map = new MapData();
    }
}