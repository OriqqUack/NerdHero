using System.Collections;
using DG.Tweening;
using UnityEngine;

public class AfterImageGeneratorPlaced : MonoBehaviour
{
    public GameObject afterimagePrefab; // 잔상 프리팹 (SpriteRenderer 포함)
    public GameObject blinkImagePrefab;
    public float interval = 0.1f;       // 잔상 생성 간격
    public float lifetime = 0.5f;       // 잔상 유지 시간
    public float fadeSpeed = 2f;        // 사라지는 속도
    
    private SpriteRenderer spriteRenderer;
    private Vector2[] positionOffsets = new Vector2[3]{new Vector2(1.7f, 1.7f), new Vector2(-1.2f, 0.5f), new Vector2(2f, -0.5f)};
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
        GameObject ai = Managers.Resource.Instantiate(afterimagePrefab, transform.position, transform.rotation);

        GameObject ob = Managers.Resource.Instantiate(blinkImagePrefab, ai.transform.position, ai.transform.rotation);
        Destroy(ob, lifetime);
        
        SpriteRenderer aiSr = ai.GetComponent<SpriteRenderer>();
        aiSr.sprite = spriteRenderer.sprite;
        aiSr.flipX = spriteRenderer.flipX;

        // HSV 색상 생성 (H만 랜덤, S/V는 고정)
        float h = Random.Range(0f, 1f); // 무지개색 중에서 랜덤
        float s = 0.8f;                 // 채도 (1에 가까울수록 선명)
        float v = 1f;                   // 명도 (1 = 밝음)

        Color hsvColor = Color.HSVToRGB(h, s, v);
        hsvColor.a = 0.6f; // 투명도 유지

        aiSr.color = hsvColor;

        aiSr.DOFade(0f, lifetime)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(ai));
    }

}
