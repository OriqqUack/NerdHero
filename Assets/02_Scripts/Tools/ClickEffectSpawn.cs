using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Canvas))]
public class ClickEffectSpawn : MonoBehaviour
{
    public GameObject clickEffectPrefab;
    private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        // PC 클릭
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
                SpawnEffect(Input.mousePosition);
        }

        // 모바일 터치
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                SpawnEffect(Input.GetTouch(0).position);
        }
    }

    void SpawnEffect(Vector2 screenPosition)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPosition,
            null,
            out localPos
        );

        GameObject effect = Managers.Resource.Instantiate(clickEffectPrefab);
        RectTransform rect = effect.GetComponent<RectTransform>();
        rect.SetParent(canvas.transform, false); // ✅ worldPositionStays = false로 해야 UI 기준으로 붙음
        rect.anchoredPosition = localPos;
        Destroy(effect, 3.0f);
    }
}