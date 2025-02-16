using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public abstract class IndicatorViewAction : ICloneable
{
    public abstract void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject,
        object range, float angle, float fillAmount);
    public abstract void HideIndicator();
    public abstract void SetFillAmount(float fillAmount);
    public abstract object Clone();
}
