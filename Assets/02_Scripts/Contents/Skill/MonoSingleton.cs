using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    private static bool isQuitting;

    public static T Instance
    {
        get
        {
            // Find�� FindObjectsInactive.Include�� ���ڷ� �־� active�� ���� ��ü�� ã�ƿ�
            if (instance == null && !isQuitting)
                instance = FindFirstObjectByType<T>(FindObjectsInactive.Include) ?? new GameObject(typeof(T).Name).AddComponent<T>();
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance != null && instance != this)
            Destroy(gameObject);
    }

    protected virtual void OnApplicationQuit() => isQuitting = true;
}
