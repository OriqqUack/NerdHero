using System;
using UnityEngine;

public class LogoScenario : MonoBehaviour
{
    [SerializeField] private Progress progress;

    private void Awake()
    {
        SystemSetup();
    }

    private void SystemSetup()
    {
        Application.runInBackground = true;
        
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
