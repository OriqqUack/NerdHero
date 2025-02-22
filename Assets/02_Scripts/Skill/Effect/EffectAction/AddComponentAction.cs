using UnityEngine;
using System;
using System.Collections.Generic;

public class AddComponentAction : EffectAction
{
    [Tooltip("스크립트를 추가할 대상 오브젝트")]
    public GameObject targetObject;

    [Tooltip("추가할 스크립트 이름 (정확한 클래스명 입력)")]
    public string scriptName;

    private Dictionary<string, Type> scriptTypeCache = new Dictionary<string, Type>();
    
    public void AttachScript()
    {
        if (targetObject == null || string.IsNullOrEmpty(scriptName))
        {
            Debug.LogWarning("대상 오브젝트 또는 스크립트 이름이 설정되지 않았습니다.");
            return;
        }

        Type scriptType = GetTypeFromName(scriptName);
        if (scriptType == null)
        {
            Debug.LogError($"스크립트 '{scriptName}'을(를) 찾을 수 없습니다.");
            return;
        }

        if (targetObject.GetComponent(scriptType) == null)
        {
            targetObject.AddComponent(scriptType);
            Debug.Log($"'{scriptName}' 스크립트가 {targetObject.name} 오브젝트에 추가되었습니다.");
        }
        else
        {
            Debug.LogWarning($"{targetObject.name} 오브젝트에 이미 '{scriptName}' 스크립트가 존재합니다.");
        }
    }

    private Type GetTypeFromName(string typeName)
    {
        if (scriptTypeCache.TryGetValue(typeName, out Type cachedType))
        {
            return cachedType;
        }

        Type foundType = Type.GetType(typeName);
        if (foundType == null)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foundType = assembly.GetType(typeName);
                if (foundType != null)
                {
                    scriptTypeCache[typeName] = foundType;
                    break;
                }
            }
        }
        return foundType;
    }

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        throw new NotImplementedException();
    }

    public override object Clone()
    {
        throw new NotImplementedException();
    }
}
