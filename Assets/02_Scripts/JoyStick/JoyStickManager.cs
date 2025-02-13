using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickManager : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform background;
    public RectTransform handle;
    private Vector2 inputVector = Vector2.zero;
    private bool isReleased = true; // 손을 뗐는지 여부

    public Vector2 GetInput() => inputVector;
    public bool IsReleased() => isReleased; // 조이스틱이 해제되었는지 확인

    public void OnDrag(PointerEventData eventData)
    {
        isReleased = false;
        Vector2 pos = eventData.position - (Vector2)background.position;
        float radius = background.sizeDelta.x / 2;
        inputVector = (pos.magnitude > radius) ? pos.normalized : pos / radius;
        handle.anchoredPosition = inputVector * radius;
    }

    public void OnPointerDown(PointerEventData eventData) => OnDrag(eventData);

    public void OnPointerUp(PointerEventData eventData)
    {
        isReleased = true;
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
