using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoSingleton<DataManager>
{
    public SaveData SaveData;

    private List<ISaveable> _saveables = new List<ISaveable>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        Init();
    }

    private void Start()
    {
        
    }

    private void OnApplicationQuit()
    {
        DataSave();
    }

    public void Init()
    {
        // 모든 ISaveable을 찾아서 리스트에 저장
        _saveables.AddRange(FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>());
        DataLoad();
    }

    public void DataSave()
    {
        foreach (var saveable in _saveables)
        {
            saveable.Save(SaveData);
        }

        ES3.Save("SaveData", SaveData, "SaveFile.txt");
        Debug.Log("💾 데이터 저장 완료");
    }

    public void DataLoad()
    {
        if (ES3.FileExists("SaveFile.txt"))
        {
            SaveData = ES3.Load<SaveData>("SaveData", "SaveFile.txt");
            foreach (var saveable in _saveables)
            {
                saveable.Load(SaveData);
            }
            Debug.Log("📦 데이터 불러오기 완료");
        }
        else
        {
            SaveData = new SaveData();
            DataSave();
        }
    }

    [ContextMenu("💣 저장 파일 삭제")]
    public void DataDelete()
    {
        if (ES3.FileExists("SaveFile.txt"))
        {
            ES3.DeleteFile("SaveFile.txt");
            Debug.Log("🧹 저장 파일 삭제 완료!");
        }
        else
        {
            Debug.Log("❗ 삭제할 저장 파일이 없습니다.");
        }
    }
}