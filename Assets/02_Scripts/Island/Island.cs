using UnityEngine;

public class Island : MonoBehaviour
{
    public SOWaveData WaveData;
    public bool IsLocked = true;
    
    static readonly Vector3 lookAtZ = new Vector3(0, 0, 1);

    
    void Update()
    {
        transform.LookAt(transform.position + lookAtZ);
    }
}
