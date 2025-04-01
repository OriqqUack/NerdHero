using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AfterimageGenerator : MonoBehaviour
{
    public GameObject afterimagePrefab; // 잔상 프리팹 (SpriteRenderer 포함)
    public float interval = 0.1f;       // 잔상 생성 간격
    public float lifetime = 0.5f;       // 잔상 유지 시간
    public float fadeSpeed = 2f;        // 사라지는 속도

    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(GenerateAfterimage());
    }

    IEnumerator GenerateAfterimage()
    {
        while (true)
        {
            CreateAfterimage();
            yield return new WaitForSeconds(interval);
        }
    }

    void CreateAfterimage()
    {
        GameObject ai = Instantiate(afterimagePrefab, transform.position, transform.rotation);
        SpriteRenderer aiSr = ai.GetComponent<SpriteRenderer>();
        aiSr.sprite = spriteRenderer.sprite;
        aiSr.flipX = spriteRenderer.flipX;
        aiSr.color = new Color(1f, 1f, 1f, 0.6f); // 초기 투명도 설정

        aiSr.DOFade(0f, lifetime)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(ai));
    }
}
