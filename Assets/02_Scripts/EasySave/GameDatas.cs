using System.Text;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

public class DataSettings
{
    public int gold = 0;
    public float bestTime = 0;
    public bool isCheck = false;
    public float[] skillTime = { 3, 6, 9 };
}

public class GameDatas : MonoBehaviour
{
    public DataSettings dataSettings = new DataSettings();

    private string fileName = "file.dat";
    private string keyName = "Data";
    
    #region SaveData
    public void SaveData()
    {
        OpenData();
    }

    public void OpenData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        
        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
            DataSource.ReadCacheOrNetwork, // 캐시에 데이터가 없거나 최신 데이터가 아니라면 네트워크를 통해 가져옴
            ConflictResolutionStrategy.UseLastKnownGood, //마지막에 정상적으로 저장된 정보를 가져옴
            OnSavedGameOpened
            );
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            var update = new SavedGameMetadataUpdate.Builder().Build(); // 이걸로 구글플레이에 저장가능
            
            //Json
            //var json = JsonUtility.ToJson(dataSettings);
            //byte[] bytes = Encoding.UTF8.GetBytes(json);
            
            //ES3
            var cache = new ES3Settings(ES3.Location.File);
            ES3.Save(keyName, dataSettings);
            byte[] bytes = ES3.LoadRawBytes(cache);
            
            savedGameClient.CommitUpdate(game, update, bytes, OnSavedGameWritten);
        }
    }

    private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("SavedGame written");
        }
        else
        {
            Debug.Log("Failed to write savegame");
        }
    }
    #endregion
    
    #region LoadData

    public void LoadData()
    {
        OpenLoadGame();
    }

    private void OpenLoadGame()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        
        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
            DataSource.ReadCacheOrNetwork, // 캐시에 데이터가 없거나 최신 데이터가 아니라면 네트워크를 통해 가져옴
            ConflictResolutionStrategy.UseLastKnownGood, //마지막에 정상적으로 저장된 정보를 가져옴
            LoadGameData
        );
    }

    private void LoadGameData(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            Debug.Log("Loaded savegame");
            
            savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
        }
        else
        {
            Debug.Log("Failed to write savegame");
        }
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] loadedData)
    {
        string data = Encoding.UTF8.GetString(loadedData);

        if (data == "")
        {
            Debug.Log("Data 없음 초기 데이터 저장");
            SaveData();
        }
        else
        {
            Debug.Log("로드 데이터 : " + data);
            
            //dataSettings = JsonUtility.FromJson<DataSettings>(data);
            var cache = new ES3Settings(ES3.Location.File);
            ES3.SaveRaw(loadedData, cache);
            ES3.LoadInto(keyName, dataSettings, cache);
        }
    }
    #endregion
    
    #region Delete

    public void DeleteData()
    {
        DeleteGameData();
    }

    private void DeleteGameData()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        
        savedGameClient.OpenWithAutomaticConflictResolution(fileName,
            DataSource.ReadCacheOrNetwork, // 캐시에 데이터가 없거나 최신 데이터가 아니라면 네트워크를 통해 가져옴
            ConflictResolutionStrategy.UseLastKnownGood, //마지막에 정상적으로 저장된 정보를 가져옴
            DeleteSaveGame
        );
    }

    private void DeleteSaveGame(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        if (status == SavedGameRequestStatus.Success)
        {
            savedGameClient.Delete(game);
            
            Debug.Log("Deleted savegame");
            ES3.DeleteFile();
        }
        else
        {
            Debug.Log("삭제 실패");
        }
    }
    #endregion
}
