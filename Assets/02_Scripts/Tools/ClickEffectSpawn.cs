using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[RequireComponent(typeof(Canvas))]
public class ClickEffectSpawn : MonoBehaviour
{
    public GameObject clickEffectPrefab;
    private Canvas canvas;
    private bool spawnEffectPending = false;
    private Vector2 pendingPosition;

    void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pendingPosition = Input.mousePosition;
            spawnEffectPending = true;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            pendingPosition = Input.GetTouch(0).position;
            spawnEffectPending = true;
        }
    }

    void LateUpdate()
    {
        if (spawnEffectPending)
        {
            CheckUIAndSpawnEffect(pendingPosition);
            spawnEffectPending = false;
        }
    }

    void CheckUIAndSpawnEffect(Vector2 screenPosition)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = screenPosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        foreach (var result in raycastResults)
        {
            if (IsBlockedUI(result.gameObject))
            {
                // ❗이펙트만 생성하지 않고 함수는 그냥 끝내
                return;
            }
        }

        // 여기까지 오면, 이펙트를 생성할 수 있다!
        SpawnEffect(screenPosition);
    }

    bool IsBlockedUI(GameObject uiObject)
    {
        // ❗ 여기 태그를 "NoEffect" 로 설정한다고 가정
        return uiObject.CompareTag("NoEffect");
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
        rect.SetParent(canvas.transform, false);
        rect.anchoredPosition = localPos;
        Destroy(effect, 3.0f);
    }
}