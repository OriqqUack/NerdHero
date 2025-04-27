using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Spotlight 영역 밖은 클릭 막고, 뚫린 곳만 클릭 통과시키는 마스크
/// Rect 영역은 UV (0~1), Circle은 Screen Space로 받음
/// </summary>
[RequireComponent(typeof(CanvasRenderer))]
public class SpotlightMask : Graphic, ICanvasRaycastFilter
{
    [Header("Circle Spotlight (Screen Space)")]
    public Vector2 circleCenter = new Vector2(960, 540); // 픽셀 단위
    public float circleRadius = 100f;                    // 픽셀 단위

    [Header("Rect Spotlight (UV Space)")]
    public Vector2 rectCenterUV = new Vector2(0.5f, 0.5f);
    public Vector2 rectSizeUV = new Vector2(0.3f, 0.1f);

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 🔵 Circle 체크 (screen space 기준)
        float dist = Vector2.Distance(sp, circleCenter);
        if (dist < circleRadius)
            return false; // 클릭 통과

        // 🔷 Rect 체크 (UV → screen space 변환)
        Vector2 rectCenterScreen = new Vector2(rectCenterUV.x * screenWidth, rectCenterUV.y * screenHeight);
        Vector2 rectSizeScreen = new Vector2(rectSizeUV.x * screenWidth, rectSizeUV.y * screenHeight);

        Vector2 diff = sp - rectCenterScreen;
        if (Mathf.Abs(diff.x) < rectSizeScreen.x * 0.5f && Mathf.Abs(diff.y) < rectSizeScreen.y * 0.5f)
            return false; // 클릭 통과

        return true; // 그 외는 클릭 막힘
    }

    // 렌더링은 하지 않음
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}