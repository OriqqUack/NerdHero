using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

//제자리에 잔상이 남는 스크립트
public class AfterImageGeneratorPlaced : MonoBehaviour
{
    public GameObject afterimagePrefab;
    public GameObject blinkImagePrefab;
    public float interval = 0.1f;
    public float lifetime = 0.5f;
    public float fadeSpeed = 2f;
    public int maxAfterimages = 2; // 동시에 유지할 잔상 수

    private SpriteRenderer spriteRenderer;
    private Queue<GameObject> afterimages = new Queue<GameObject>();

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
        Destroy(ob, 1f);

        SpriteRenderer aiSr = ai.GetComponent<SpriteRenderer>();
        aiSr.sprite = spriteRenderer.sprite;
        aiSr.flipX = spriteRenderer.flipX;

        float h = Random.Range(0f, 1f);
        float s = 0.8f;
        float v = 1f;

        Color hsvColor = Color.HSVToRGB(h, s, v);
        hsvColor.a = 0.6f;
        aiSr.color = hsvColor;

        aiSr.DOFade(0f, lifetime)
            .SetEase(Ease.Linear)
            .OnComplete(() => {
                afterimages.Dequeue(); // 큐에서 제거
                Destroy(ai);
            });

        afterimages.Enqueue(ai);

        // 기존 잔상이 2개 초과면 가장 오래된 것 즉시 제거
        if (afterimages.Count > maxAfterimages)
        {
            GameObject oldest = afterimages.Dequeue();
            if (oldest != null)
            {
                Destroy(oldest);
            }
        }
    }
}