using System;
using UnityEngine;

public class Island : MonoBehaviour
{
    public SOWaveData WaveData;
    public bool IsLocked = true;
    
    static readonly Vector3 lookAtZ = new Vector3(0, 0, 1);

    [HideInInspector] public LockedEffect LockedEffect;
    
    private void Awake()
    {
        LockedEffect = transform.GetChild(0).GetComponent<LockedEffect>();
    }


    void Update()
    {
        transform.LookAt(transform.position + lookAtZ);
    }
}
