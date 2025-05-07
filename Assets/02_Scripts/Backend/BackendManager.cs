using System;
using UnityEngine;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
        BackendSetup();
    }


    private void BackendSetup()
    {
        
    }

}
