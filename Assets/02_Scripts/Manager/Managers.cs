using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Managers : MonoSingleton<Managers>
{
    public static bool IsDestroying = false;
    #region Core
    //InputManager _input = new InputManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    EnergyManager _energy = new EnergyManager();
    SoundManager _sound = new SoundManager();
    DataManager _data = new DataManager();
    InventoryManager _inventory = new InventoryManager();

    //public static InputManager Input { get { return Instance._input; } }
    public static PoolManager Pool { get { return Instance._pool; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static EnergyManager EnergyManager { get { return Instance._energy; } }
    public static SoundManager SoundManager { get { return Instance._sound; } }
    public static DataManager DataManager { get { return Instance._data; } }
    public static InventoryManager InventoryManager { get { return Instance._inventory; } }
    #endregion

    private List<Coroutine> _activeCoroutines = new List<Coroutine>();

    protected override void Awake()
    {
        base.Awake();
        Init();
    }

    static void Init()
    {
        if (instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            instance = go.GetComponent<Managers>();

            instance._pool.Init();
            instance._sound.Init();
            instance._inventory.Init();
            instance._energy.Init();
            
            instance._data.Init();
        }		
    }

    public Coroutine StartManagedCoroutine(IEnumerator routine)
    {
        Coroutine co = StartCoroutine(routine);
        _activeCoroutines.Add(co);
        return co;
    }

    public void StopManagedCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            _activeCoroutines.Remove(coroutine);
        }
    }
    
    public void StopAllManagedCoroutines()
    {
        foreach (var co in _activeCoroutines)
        {
            if (co != null)
                StopCoroutine(co);
        }
        _activeCoroutines.Clear();
    }
    
    public static void Clear()
    {
        //Input.Clear();
        Pool.Clear();
    }

    private void OnApplicationQuit()
    {
        DataManager.DataSave();
        IsDestroying = true;
        Clear();
    }
    
    [ContextMenu("💣 저장 파일 삭제")]
    public void DataDelete()
    {
        if (ES3.FileExists("SaveFile.es3"))
        {
            ES3.DeleteFile("SaveFile.es3");
            Debug.Log("🧹 저장 파일 삭제 완료!");
        }
        else
        {
            Debug.Log("❗ 삭제할 저장 파일이 없습니다.");
        }
    }
}