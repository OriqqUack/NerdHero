using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BackEnd;
using BackEnd.BackndNewtonsoft.Json;
using LitJson;
using UnityEngine.SceneManagement;

[System.Serializable] 
public class GameData
{
    public int score, gold, gem, highScore, playerLevel, currentIslandIndex, currentEnergy, currentExp;
    public int totalStageClear, ranking, totalKills, victories;
    public List<int> inventoryItems = new List<int>();
    public List<bool> clearedMap = new List<bool>();
    public int equippedWeapon, equippedArmor, equippedHelmet, equippedBoots;
    public string energyLastCharge, profileFrameName, profileIconName;
    public string playerName, playerId;
    public bool isClearedTutorial, isCleared;
}

public class BackendDataManager
{
    public GameData gameData;

    private List<ISaveable> _saveables = new List<ISaveable>();

    public void LoadUserData()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        gameData = new GameData();
        _saveables.AddRange(Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());

        var bro = Backend.GameData.GetMyData("UserData", new Where());

        if (!bro.IsSuccess())
        {
            Debug.LogWarning($"Data를 가져올 수 없습니다 : {bro.GetMessage()}");
            return;
        }

        JsonData rows = bro.FlattenRows();

        if (rows.Count == 0)
        {
            // 신규 유저: 기본값 세팅 후 Insert
            DataInit();
            InsertNewUserData();
        }
        else
        {
            gameData.clearedMap = ParseJsonList<bool>(rows[0]["clearedMap"]);
            gameData.inventoryItems = ParseJsonList<int>(rows[0]["inventoryItems"]);
            
            // 기존 유저: 값 불러오기
            gameData.score = int.Parse(rows[0]["score"].ToString());
            gameData.gold = int.Parse(rows[0]["gold"].ToString());
            gameData.gem = int.Parse(rows[0]["gem"].ToString());
            gameData.highScore = int.Parse(rows[0]["highScore"].ToString());
            gameData.playerLevel = int.Parse(rows[0]["playerLevel"].ToString());
            gameData.currentIslandIndex = int.Parse(rows[0]["currentIslandIndex"].ToString());
            gameData.currentEnergy = int.Parse(rows[0]["currentEnergy"].ToString());
            gameData.currentExp = int.Parse(rows[0]["currentExp"].ToString());
            gameData.totalKills = int.Parse(rows[0]["totalKills"].ToString());
            gameData.victories = int.Parse(rows[0]["victories"].ToString());
            gameData.totalStageClear = int.Parse(rows[0]["totalStageClear"].ToString());
            gameData.ranking = int.Parse(rows[0]["ranking"].ToString());

            gameData.equippedWeapon = int.Parse(rows[0]["equippedWeapon"].ToString());
            gameData.equippedArmor = int.Parse(rows[0]["equippedArmor"].ToString());
            gameData.equippedHelmet = int.Parse(rows[0]["equippedHelmet"].ToString());
            gameData.equippedBoots = int.Parse(rows[0]["equippedBoots"].ToString());
            
            gameData.energyLastCharge = rows[0]["energyLastCharge"].ToString();
            gameData.profileFrameName = rows[0]["profileFrameName"].ToString();
            gameData.profileIconName = rows[0]["profileIconName"].ToString();
            
            gameData.playerName = rows[0]["playerName"].ToString();
            gameData.playerId = rows[0]["playerId"].ToString();
            
            gameData.isClearedTutorial = bool.Parse(rows[0]["isClearedTutorial"].ToString());
            gameData.isCleared = bool.Parse(rows[0]["isCleared"].ToString());

            Debug.Log("기존 유저 데이터 로드 완료");
        }

        DataLoad();
    }

    public void AddSavable(ISaveable saveable)
    {
        _saveables.Add(saveable);
    }
    
    private List<T> ParseJsonList<T>(JsonData jsonArray)
    {
        List<T> list = new List<T>();

        // 배열이 아닌 경우, null 또는 초기화 안된 JsonData 방지
        if (jsonArray == null || !jsonArray.IsArray)
        {
            Debug.LogWarning("⚠️ JsonData가 배열이 아니거나 초기화되지 않았습니다.");
            return list;
        }

        foreach (JsonData item in jsonArray)
        {
            if (typeof(T) == typeof(int))
            {
                list.Add((T)(object)(int)item);
            }
            else if (typeof(T) == typeof(bool))
            {
                list.Add((T)(object)(bool)item);
            }
            else if (typeof(T) == typeof(string))
            {
                list.Add((T)(object)(string)item);
            }
            else
            {
                Debug.LogError($"❌ ParseJsonList: 타입 {typeof(T)} 지원 안됨");
                break;
            }
        }
        return list;
    }

    
    private void DataInit()
    {
        gameData.inventoryItems = new List<int>();
        gameData.equippedWeapon = 0;
        gameData.equippedArmor = 0;
        gameData.equippedHelmet = 0;
        gameData.equippedBoots = 0;
        gameData.energyLastCharge = string.Empty;
        gameData.profileFrameName = string.Empty;
        gameData.profileIconName = string.Empty;
        gameData.score = 0;
        gameData.gold = 100;
        gameData.gem = 1000;
        gameData.highScore = 0;
        gameData.playerLevel = 1;
        gameData.currentIslandIndex = -1;
        gameData.currentEnergy = 100;
        gameData.currentExp = 0;
        gameData.totalKills = 0;
        gameData.victories = 0;
        gameData.totalStageClear = 0;
        gameData.ranking = 0;
        gameData.isClearedTutorial = false;
        gameData.isCleared = false;
    }

    void InsertNewUserData()
    {
        Param param = new Param();
        param.Add("inventoryItems", JsonConvert.SerializeObject(gameData.inventoryItems));
        param.Add("clearedMap", JsonConvert.SerializeObject(gameData.clearedMap));

        param.Add("equippedWeapon", gameData.equippedWeapon);
        param.Add("equippedArmor", gameData.equippedArmor);
        param.Add("equippedHelmet", gameData.equippedHelmet);
        param.Add("equippedBoots", gameData.equippedBoots);
        param.Add("playerLevel", gameData.playerLevel);
        param.Add("score", gameData.score);
        param.Add("gold", gameData.gold);
        param.Add("gem", gameData.gem);
        param.Add("highScore", gameData.highScore);
        param.Add("currentIslandIndex", gameData.currentIslandIndex);
        param.Add("currentEnergy", gameData.currentEnergy);
        param.Add("currentExp", gameData.currentExp);
        param.Add("totalKills", gameData.totalKills);
        param.Add("victories", gameData.victories);
        param.Add("totalStageClear", gameData.totalStageClear);
        param.Add("ranking", gameData.ranking);
        
        param.Add("energyLastCharge", gameData.energyLastCharge);
        param.Add("profileFrameName", gameData.profileFrameName);
        param.Add("profileIconName", gameData.profileIconName);

        param.Add("playerName", gameData.playerName);
        param.Add("playerId", gameData.playerId);
        
        param.Add("isClearedTutorial", gameData.isClearedTutorial);
        param.Add("isCleared", gameData.isCleared);
        
        var bro = Backend.GameData.Insert("UserData", param);

        if (bro.IsSuccess())
        {
            Debug.Log("신규 유저 데이터 저장 성공");
        }
        else
        {
            Debug.LogError("데이터 저장 실패: " + bro.GetMessage());
        }
    }

    public void UpdateField<T>(string fieldName, T value)
    {
        Param param = new Param();
        param.Add(fieldName, value);

        Where where = new Where();
        where.Equal("owner_inDate", Backend.UserInDate);

        var bro = Backend.GameData.Update("UserData", where, param);

        if (bro.IsSuccess())
            Debug.Log($"{fieldName} 업데이트 성공");
        else
            Debug.LogError($"{fieldName} 업데이트 실패: {bro.GetMessage()}");
    }
    
    public void UpdateField<T>(string fieldName, List<T> list)
    {
        Param param = new Param();

        param.Add(fieldName, list); // 이렇게 하면 Backend가 JSON으로 자동 직렬화해 줌

        Where where = new Where();
        where.Equal("owner_inDate", Backend.UserInDate);

        var bro = Backend.GameData.Update("UserData", where, param);

        if (bro.IsSuccess())
            Debug.Log($"✅ {fieldName} 리스트 업데이트 성공");
        else
            Debug.LogError($"❌ {fieldName} 리스트 업데이트 실패: {bro.GetMessage()}");
    }

    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _saveables.Clear();
        _saveables.AddRange(Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());
        DataLoad();
    }
    
    public void DataLoad()
    {
        foreach (var saveable in _saveables)
        {
            saveable.Load(gameData);
        }
    }
}
