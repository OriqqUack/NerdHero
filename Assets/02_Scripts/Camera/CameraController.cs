using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // 따라갈 대상 (플레이어)
    [SerializeField] private float smoothSpeed = 5f; // 부드러운 이동 속도
    [SerializeField] private bool lockX; // X축 고정 여부
    [SerializeField] private bool lockY; // Y축 고정 여부
    [SerializeField] private Vector2 screenSize = new Vector2(1920, 1080); // 기본 화면 너비

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        SetCameraSize(screenSize);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position;

        if (lockX) targetPosition.x = transform.position.x; // X축 고정
        if (lockY) targetPosition.y = transform.position.y; // Y축 고정

        // 부드러운 카메라 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }

    // 카메라의 크기를 조절하는 함수
    public void SetCameraSize(Vector2 screenSize)
    {
        float aspectRatio = screenSize.x / screenSize.y;
        cam.orthographicSize = screenSize.y / 200f; // 100f = 기본 비율 조정 값
        Debug.Log($"카메라 크기 조정: {screenSize.x} x {screenSize.y}, 비율: {aspectRatio}");
    }
}