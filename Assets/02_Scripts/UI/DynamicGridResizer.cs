using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class DynamicGridResizer : MonoBehaviour
{
    public int columns = 8;
    public Vector2 spacing = new Vector2(16, 16);
    public Vector2 padding = new Vector2(0, 0); // 왼쪽/오른쪽 패딩 (추가로 설정할 경우)

    private RectTransform rectTransform;
    private GridLayoutGroup gridLayout;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        gridLayout = GetComponent<GridLayoutGroup>();

        UpdateCellSize();
    }

    void UpdateCellSize()
    {
        float totalWidth = rectTransform.rect.width;

        // 가용 너비 계산: 전체 너비 - (spacing * (열-1)) - (좌우 padding)
        float usableWidth = totalWidth - spacing.x * (columns - 1) - padding.x * 2;

        float cellWidth = usableWidth / columns;

        // 정사각형 셀 만들기 (원하면 높이를 따로 설정해도 됨)
        Vector2 newCellSize = new Vector2(cellWidth, cellWidth);

        gridLayout.cellSize = newCellSize;
        gridLayout.spacing = spacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = columns;
        gridLayout.padding = new RectOffset((int)padding.x, (int)padding.x, (int)padding.y, (int)padding.y);
    }

    // 크기 바뀔 때 자동 반영하고 싶으면 아래처럼 해도 됨
    void OnRectTransformDimensionsChange()
    {
        if (rectTransform != null)
            UpdateCellSize();
    }
}