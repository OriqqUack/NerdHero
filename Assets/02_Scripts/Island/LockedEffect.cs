using DG.Tweening;
using UnityEngine;

public class LockedEffect : MonoBehaviour
{
    public GameObject breakVFXPrefab; // 터질 때 나올 VFX 프리팹
    public float shakeAngle = 20f; // 흔들리는 각도
    public int shakeCount = 20; // 흔들릴 횟수 (왕복 기준)
    public float shakeDuration = 0.1f; // 한 번 흔들리는 시간

    private SpriteRenderer parentSpriteRenderer;
    private Island parentIsland;
    
    private void Start()
    {
        if (transform.parent != null)
        {
            parentSpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
            parentIsland = parentSpriteRenderer.GetComponent<Island>();

            if (parentIsland.IsLocked)
            {
                gameObject.SetActive(true);
                parentSpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
            else
            {
                gameObject.SetActive(false);
                parentSpriteRenderer.color = Color.white;
            }
        }
    }

    public void ShakeAndBreak()
    {
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < shakeCount; i++)
        {
            float targetAngle = (i % 2 == 0) ? shakeAngle : -shakeAngle;
            seq.Append(transform.DORotate(new Vector3(0, 0, targetAngle), shakeDuration).SetRelative(false));
        }

        seq.Append(transform.DORotate(Vector3.zero, shakeDuration / 2)); // 마지막엔 정중앙으로
        
        seq.AppendCallback(() =>
        {
            if (parentSpriteRenderer != null)
            {
                // 부모의 색깔을 흰색으로 천천히 바꿔주기 (0.5초 동안)
                parentSpriteRenderer.DOColor(Color.white, 0.5f);
            }
        });

        seq.AppendCallback(ExplodeLock); // 터뜨리기
    }

    private void ExplodeLock()
    {
        if (breakVFXPrefab != null)
        {
            Instantiate(breakVFXPrefab, transform.position, Quaternion.identity);
        }

        ActiveButton();
        Destroy(transform.gameObject); // 자물쇠 파괴
    }

    private void ActiveButton()
    {
        parentSpriteRenderer.GetComponent<Island>().IsLocked = false;
        CircleExpositor.Instance.EnterButtonActive(true);
    }
}
