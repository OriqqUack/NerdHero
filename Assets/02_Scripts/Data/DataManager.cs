using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class DataManager
{
    /*public SaveData SaveData;

    private List<ISaveable> _saveables = new List<ISaveable>();
    private ES3Settings _es3Settings;
    private static readonly string _myPassword = "a7244290a";
    public void Init()
    {
        // 모든 ISaveable을 찾아서 리스트에 저장
        SceneManager.sceneLoaded += OnSceneLoaded;
        _saveables.AddRange(Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());
        _es3Settings = new ES3Settings(ES3.EncryptionType.AES, _myPassword);
        
        DataLoad();
    }

    public void AddSaveable(ISaveable saveable)
    {
        _saveables.Add(saveable);
    }

    public void DataSave()
    {
        foreach (var saveable in _saveables)
        {
            saveable.Save(SaveData);
        }

        ES3.Save("SaveData", SaveData, _es3Settings.path);
        Debug.Log("💾 데이터 저장 완료");
    }

    public void DataLoad()
    {
        if (ES3.FileExists(_es3Settings.path))
        {
            SaveData = ES3.Load<SaveData>("SaveData", _es3Settings.path);
            foreach (var saveable in _saveables)
            {
                saveable.Load(SaveData);
            }
            Debug.Log("📦 데이터 불러오기 완료");
        }
        else
        {
            SaveData = new SaveData();
            foreach (var saveable in _saveables)
            {
                saveable.Load(SaveData);
            }
            DataSave();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _saveables.Clear();
        _saveables.AddRange(Object.FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());
        DataLoad();
    }*/
}