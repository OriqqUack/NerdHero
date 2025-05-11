using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIRaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 모바일이면 Input.touchCount 체크도 가능
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count == 0)
            {
                Debug.Log("❌ 아무 UI도 감지되지 않았습니다.");
            }
            else
            {
                Debug.Log($"🎯 클릭된 UI 목록 ({results.Count}개):");
                foreach (var result in results)
                {
                    Debug.Log($"👉 {result.gameObject.name} (Layer: {result.gameObject.layer})");
                }
            }
        }
    }
}