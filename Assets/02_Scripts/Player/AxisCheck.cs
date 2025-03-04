using System;
using UnityEngine;

public class AxisCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
            other.GetComponent<EntityHUD>().AxisImageControl(true);
    }
    
    private void OnTriggerExit(Collider other)
    {
        EntityHUD hud = other.GetComponent<EntityHUD>();
        if (hud)
            other.GetComponent<EntityHUD>().AxisImageControl(false);
    }
}
