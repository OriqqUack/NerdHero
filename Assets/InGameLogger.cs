using UnityEngine;
using TMPro;

public class InGameLogger : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    private static InGameLogger instance;

    private void Awake()
    {
        instance = this;
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
#if !UNITY_EDITOR
        if (debugText == null) return;

        debugText.text += $"[{type}] {logString}\n";
        if (debugText.text.Length > 5000)
        {
            debugText.text = debugText.text.Substring(debugText.text.Length - 5000);
        }
#endif
    }

    public static void Log(string message)
    {
        if (instance != null && instance.debugText != null)
        {
            instance.debugText.text += $"[Custom] {message}\n";
        }
    }
}