using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf('/');
            if (index >= 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }

        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        if (original.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(original, parent).gameObject;

        GameObject go = Object.Instantiate(original, parent);
        go.name = original.name;
        return go;
    }
    
    public GameObject Instantiate(GameObject gameObject, Transform parent = null)
    {
        if (gameObject.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(gameObject, parent).gameObject;

        GameObject go = Object.Instantiate(gameObject, parent);
        go.name = gameObject.name;
        return go;
    }
    
    public GameObject Instantiate(GameObject gameObject, Vector3 location)
    {
        if (gameObject.GetComponent<Poolable>() != null)
            return Managers.Pool.Pop(gameObject, null).gameObject;

        GameObject go = Object.Instantiate(gameObject, location, Quaternion.identity);
        go.name = gameObject.name;
        return go;
    }

    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation , Transform parent = null, int count = 5 )
    {
        GameObject original = Load<GameObject>($"Prefabs/{path}");
        if (original == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        GameObject go;
        if (original.GetComponent<Poolable>() != null)
        {
            go = Managers.Pool.Pop(original, parent, count).gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
        }
        else
        {
            go = Object.Instantiate(original, position, rotation);
        }
        return go;
    }
    
    public GameObject Instantiate(GameObject original, Vector3 position, Quaternion rotation , Transform parent = null, int count = 5 )
    {
        GameObject go;
        if (original.GetComponent<Poolable>() != null)
        {
            go = Managers.Pool.Pop(original, parent, count).gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
        }
        else
        {
            go = Object.Instantiate(original, position, rotation);
        }
        return go;
    }

    public void Destroy(GameObject go, float delay = 0f)
    {
        if (go == null)
            return;

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Push(poolable);
            return;
        }

        Object.Destroy(go, delay);
    }
}
