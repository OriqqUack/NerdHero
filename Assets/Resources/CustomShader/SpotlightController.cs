using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpotlightController : MonoSingleton<SpotlightController>
{
    [Header("Target UI")]
    [SerializeField] private RawImage spotlightImage;
    [SerializeField] private RectTransform rectTarget;
    [SerializeField] private RectTransform circleTarget;

    [Header("Optional")]
    [SerializeField] private SpotlightMask raycastMask; // ⬅ 연결 대상 추가

    [Header("Offset")]
    [SerializeField] private float rectOffset = 20f;
    [SerializeField] private float circleOffset = 30f;
    [SerializeField] private float smoothSpeed = 10f;
    
    // 이전 프레임에서 유지되는 값 저장용
    private Vector2 smoothedRectCenterUV;
    private Vector2 smoothedRectSizeUV;
    private Vector2 smoothedCircleCenter;
    private float smoothedCircleRadius;
    
    private Material _mat;
    private bool _canClick;
    void Start()
    {
        _mat = spotlightImage.material;
    }

    void Update()
    {
        Vector2 rectCenterUV, rectSizeUV;
        Vector2 circleCenter;
        float circleRadius;

        // ➤ spotlight 위치 계산
        CalculateRectSpotlight(out rectCenterUV, out rectSizeUV);
        CalculateCircleSpotlight(out circleCenter, out circleRadius);

        // ➤ 자연스럽게 보간
        smoothedRectCenterUV = Vector2.Lerp(smoothedRectCenterUV, rectCenterUV, Time.unscaledDeltaTime * smoothSpeed);
        smoothedRectSizeUV   = Vector2.Lerp(smoothedRectSizeUV, rectSizeUV, Time.unscaledDeltaTime * smoothSpeed);
        smoothedCircleCenter = Vector2.Lerp(smoothedCircleCenter, circleCenter, Time.unscaledDeltaTime * smoothSpeed);
        smoothedCircleRadius = Mathf.Lerp(smoothedCircleRadius, circleRadius, Time.unscaledDeltaTime * smoothSpeed);

        // ➤ 쉐이더에 적용
        _mat.SetVector("_RectCenter", new Vector4(smoothedRectCenterUV.x, smoothedRectCenterUV.y, 0, 0));
        _mat.SetVector("_RectSize", new Vector4(smoothedRectSizeUV.x, smoothedRectSizeUV.y, 0, 0));
        _mat.SetVector("_CircleCenter", new Vector4(smoothedCircleCenter.x, smoothedCircleCenter.y, 0, 0));
        _mat.SetFloat("_CircleRadius", smoothedCircleRadius);

        // ➤ Raycast 마스크에 값 전달
        if (raycastMask != null && _canClick)
        {
            raycastMask.rectCenterUV = smoothedRectCenterUV;
            raycastMask.rectSizeUV = smoothedRectSizeUV;
            raycastMask.circleCenter = smoothedCircleCenter;
            raycastMask.circleRadius = smoothedCircleRadius;
        }
        else
        {
            raycastMask.rectCenterUV = Vector2.zero;
            raycastMask.rectSizeUV = Vector2.zero;
            raycastMask.circleCenter = Vector2.zero;
            raycastMask.circleRadius = 0f;
        }
    }

    public void SetTarget(Transform rectTarget, Transform circleTarget, bool canClick = true)
    {
        spotlightImage.gameObject.SetActive(true);
        raycastMask.gameObject.SetActive(true);
        if (rectTarget != null)
            this.rectTarget = rectTarget.GetComponent<RectTransform>();
        else
            this.rectTarget = null;
        
        if (circleTarget != null)
            this.circleTarget = circleTarget.GetComponent<RectTransform>();
        else
            this.circleTarget = null;
        
        _canClick = canClick;
    }

    public void OffSpotlight()
    {
        spotlightImage.gameObject.SetActive(false);
        raycastMask.gameObject.SetActive(false);
        rectTarget = null;
        circleTarget = null;
    }
    
    void CalculateRectSpotlight(out Vector2 uvCenter, out Vector2 uvSize)
    {
        if (!rectTarget)
        {
            uvCenter = default;
            uvSize = default;
            return;
        }

        Vector3[] corners = new Vector3[4];
        rectTarget.GetWorldCorners(corners);

        Vector2 bl = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        Vector2 tr = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        bl -= Vector2.one * rectOffset;
        tr += Vector2.one * rectOffset;

        Vector2 center = (bl + tr) * 0.5f;
        Vector2 size = tr - bl;

        uvCenter = new Vector2(center.x / Screen.width, center.y / Screen.height);
        uvSize = new Vector2(size.x / Screen.width, size.y / Screen.height);
    }

    void CalculateCircleSpotlight(out Vector2 screenCenter, out float radius)
    {
        if (!circleTarget)
        {
            screenCenter = default;
            radius = 0;
            return;
        }

        Vector3[] corners = new Vector3[4];
        circleTarget.GetWorldCorners(corners);

        Vector2 bl = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        Vector2 tr = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        screenCenter = (bl + tr) * 0.5f;
        Vector2 size = tr - bl;

        float baseRadius = Mathf.Min(size.x, size.y) * 0.5f;
        radius = baseRadius + circleOffset;
    }
}