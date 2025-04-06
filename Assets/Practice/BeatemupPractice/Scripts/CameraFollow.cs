using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Margins & Smoothing")]
    public float xMargin = 1f;
    public float xSmooth = 8f;
    public float ySmooth = 8f;

    [Header("Camera Bounds")]
    public Vector2 minXAndY;
    public Vector2 maxXAndY;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = WaveManager.Instance.PlayerTransform;
    }

    private bool IsBeyondXMargin()
    {
        float diff = transform.position.x - playerTransform.position.x;
        return diff < -xMargin || diff > xMargin;
    }

    private void LateUpdate()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos;

        // x축 위치 보정
        if (IsBeyondXMargin())
        {
            targetPos.x = Mathf.Lerp(currentPos.x, playerTransform.position.x, xSmooth * Time.deltaTime);
            targetPos.x = Mathf.Clamp(targetPos.x, minXAndY.x, maxXAndY.x);
        }

        // 추후 y축 활성화 시 아래 코드 주석 해제
        // if (IsBeyondYMargin())
        // {
        //     targetPos.y = Mathf.Lerp(currentPos.y, playerTransform.position.y, ySmooth * Time.deltaTime);
        //     targetPos.y = Mathf.Clamp(targetPos.y, minXAndY.y, maxXAndY.y);
        // }

        transform.position = new Vector3(targetPos.x, currentPos.y, currentPos.z);
    }
}