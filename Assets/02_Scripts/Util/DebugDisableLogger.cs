using UnityEngine;

public class DebugDisableLogger : MonoBehaviour
{
    private void OnDisable()
    {
        Debug.Log($"[{gameObject.name}] Disabled! \n{System.Environment.StackTrace}");
    }

    private void OnEnable()
    {
        Debug.Log($"[{gameObject.name}] Disabled! \n{System.Environment.StackTrace}");
    }
}