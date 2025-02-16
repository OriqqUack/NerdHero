using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public abstract class TargetSearchAction : ICloneable
{
    [Header("Indicator")]
    [SerializeField] private bool isShowIndicatorPlayerOnly;
    [SerializeReference, SubclassSelector]
    private IndicatorViewAction indicatorViewAction;

    [Header("Option")]
    [SerializeField] private bool isUseScale;

    private float scale;

    public float Scale
    {
        get => scale;
        set
        {
            if (scale == value)
                return;

            scale = value;
            indicatorViewAction?.SetFillAmount(scale);
            OnScaleChanged(scale);
        }
    }

    public abstract object Range { get; }
    public abstract object ScaledRange { get; }
    public abstract float Angle { get; }
    public object ProperRange => isUseScale ? ScaledRange : Range;

    public bool IsUseScale => isUseScale;

    public TargetSearchAction() { }

    public TargetSearchAction(TargetSearchAction copy)
    { 
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }

    public abstract TargetSearchResult Search(TargetSearcher targetSearcher, Entity requesterEntity,
        GameObject requesterObject, TargetSelectionResult selectResult);

    public abstract object Clone();

    public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject, float fillAmount)
    {
        var entity = requesterObject.GetComponent<Entity>();
        if (isShowIndicatorPlayerOnly && (entity == null || !entity.IsPlayer))
            return;

        indicatorViewAction?.ShowIndicator(targetSearcher, requesterObject, Range, Angle, fillAmount);
    }

    public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    public string BuildDescription(string description, string prefixKeyword) 
        => TextReplacer.Replace(description, prefixKeyword + ".searchAction", GetStringsByKeyword());

    protected virtual IReadOnlyDictionary<string, string> GetStringsByKeyword() => null;

    protected virtual void OnScaleChanged(float newScale) { }
}
