using UnityEngine;

public class CloudScroll : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 1f;       // 움직이는 속도
    [SerializeField] private float resetPositionX = 10f;   // 이 위치 이상 가면 초기화
    [SerializeField] private float startPositionX = -10f;  // 되돌릴 위치

    private void FixedUpdate()
    {
        transform.position += Vector3.left * scrollSpeed * Time.deltaTime;

        if (transform.position.x <= resetPositionX)
        {
            Vector3 pos = transform.position;
            pos.x = startPositionX;
            transform.position = pos;
        }
    }
}
