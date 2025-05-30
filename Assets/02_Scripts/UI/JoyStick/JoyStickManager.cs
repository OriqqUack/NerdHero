using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStickManager : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private RectTransform touchePanel;
    
    private Vector2 _inputVector = Vector2.zero;
    private bool _isReleased = true;
    private Canvas _canvas; // UI 캔버스 참조

    public Vector2 GetInput() => _inputVector;
    public bool IsReleased() => _isReleased;

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        background.gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _isReleased = false;
        Vector2 pos = eventData.position - (Vector2)background.position;
        float radius = background.sizeDelta.x / 2;
        _inputVector = (pos.magnitude > radius) ? pos.normalized : pos / radius;
        handle.anchoredPosition = _inputVector * radius;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        background.gameObject.SetActive(true); // 조이스틱 활성화
        background.position = eventData.position; // 터치한 위치에 조이스틱 배치
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isReleased = true;
        _inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
        background.gameObject.SetActive(false); // 손을 떼면 조이스틱 숨김
    }

    private void SetTouchPanelSize()
    {
        touchePanel.sizeDelta = new Vector2(Screen.width/2, Screen.height);
    }
}