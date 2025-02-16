using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircleIndicatorViewAction : IndicatorViewAction
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private float indicatorRadiusOverride;
    [SerializeField] private float indicatorAngleOverride;
    [SerializeField] private bool isUseIndicatorFillAmount;
    [SerializeField] private bool isAttachIndicatorToRequester;

    private Indicator spawnedRangeIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject,
        object range, float angle, float fillAmount)
    {
        Debug.Assert(range is float, "CircleIndicatorViewAction::ShowIndicator - range�� null �Ǵ� float���� ���˴ϴ�.");

        HideIndicator();

        fillAmount = isUseIndicatorFillAmount ? fillAmount : 0f;
        var attachTarget = isAttachIndicatorToRequester ? requesterObject.transform : null;
        float radius = Mathf.Approximately(indicatorRadiusOverride, 0f) ? (float)range : indicatorRadiusOverride;
        angle = Mathf.Approximately(indicatorAngleOverride, 0f) ? angle : indicatorAngleOverride;

        spawnedRangeIndicator = GameObject.Instantiate(indicatorPrefab).GetComponent<Indicator>();
        spawnedRangeIndicator.Setup(angle, radius, fillAmount, attachTarget);
    }

    public override void HideIndicator()
    {
        if (!spawnedRangeIndicator)
            return;

        GameObject.Destroy(spawnedRangeIndicator.gameObject);
    }

    public override void SetFillAmount(float fillAmount)
    {
        if (!isUseIndicatorFillAmount || spawnedRangeIndicator == null)
            return;

        spawnedRangeIndicator.FillAmount = fillAmount;
    }

    public override object Clone()
    {
        return new CircleIndicatorViewAction()
        {
            indicatorPrefab = indicatorPrefab,
            indicatorAngleOverride = indicatorAngleOverride,
            indicatorRadiusOverride = indicatorRadiusOverride,
            isUseIndicatorFillAmount = isUseIndicatorFillAmount,
            isAttachIndicatorToRequester = isAttachIndicatorToRequester
        };
    }
}
