using PixelCrushers.DialogueSystem;
using UnityEngine;

public static class QuestVariableManager
{
    /// <summary>
    /// 퀘스트 관련 변수를 확인하고 필요하면 기본값으로 초기화합니다.
    /// </summary>
    /// <param name="questID">퀘스트 고유 ID</param>
    public static void EnsureQuestVariables(string questID)
    {
        string statusVariable = $"{questID}_Status";
        string progressVariable = $"{questID}_Progress";

        InitializeVariable(statusVariable, "Started");
        InitializeVariable(progressVariable, 0);
    }

    /// <summary>
    /// 변수를 초기화합니다. 변수가 존재하지 않으면 기본값으로 설정합니다.
    /// </summary>
    /// <param name="variableName">변수 이름</param>
    /// <param name="defaultValue">초기값</param>
    private static void InitializeVariable(string variableName, object defaultValue)
    {
        // 변수 값 가져오기 (string, number, bool 타입만 존재)
        var currentValue = DialogueLua.GetVariable(variableName).AsString; // 존재 여부 확인용
        if (string.IsNullOrEmpty(currentValue)) // 변수가 존재하지 않는다면 초기화
        {
            DialogueLua.SetVariable(variableName, defaultValue);
            Debug.Log($"[QuestVariableManager] Variable '{variableName}' initialized with default value: {defaultValue}");
        }
        else
        {
            UpdateVariable(variableName, defaultValue);
        }
    }

    /// <summary>
    /// 변수 값을 업데이트합니다.
    /// </summary>
    /// <param name="variableName">변수 이름</param>
    /// <param name="value">새 값</param>
    public static void UpdateVariable(string variableName, object value)
    {
        DialogueLua.SetVariable(variableName, value);
        Debug.Log($"[QuestVariableManager] Variable '{variableName}' updated to: {value}");
    }

    /// <summary>
    /// 변수 값을 가져옵니다.
    /// </summary>
    /// <param name="variableName">변수 이름</param>
    /// <returns>변수 값</returns>
    public static object GetVariable(string variableName)
    {
        var luaResult = DialogueLua.GetVariable(variableName);

        // Lua 변수는 string, float, bool 중 하나로 반환
        if (luaResult.IsBool) return luaResult.AsBool;
        if (luaResult.IsNumber) return luaResult.AsFloat;
        if (luaResult.IsString) return luaResult.AsString;

        Debug.LogWarning($"[QuestVariableManager] Variable '{variableName}' is of an unknown type.");
        return null;
    }

    /// <summary>
    /// 특정 퀘스트에 연관된 변수들을 모두 초기화합니다.
    /// </summary>
    /// <param name="questID">퀘스트 고유 ID</param>
    public static void ResetQuestVariables(string questID)
    {
        string statusVariable = $"{questID}_Status";
        string progressVariable = $"{questID}_Progress";

        UpdateVariable(statusVariable, "NotStarted");
        UpdateVariable(progressVariable, 0);
        Debug.Log($"[QuestVariableManager] Quest variables for '{questID}' have been reset.");
    }
}
