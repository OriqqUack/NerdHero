using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AfterimageGenerator : MonoBehaviour
{
    public GameObject afterimagePrefab; // 잔상 프리팹 (SpriteRenderer 포함)
    public GameObject blinkImagePrefab;
    public float interval = 0.1f;       // 잔상 생성 간격

    private List<GameObject> afterimageList = new List<GameObject>();
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(GenerateAfterimage());
        
        GameObject blink = Managers.Resource.Instantiate(blinkImagePrefab, transform);
        blink.transform.position = transform.position;
    }

    IEnumerator GenerateAfterimage()
    {
        for (int i = 1; i < 3; i++)
        {
            CreateAfterimage(i);
            yield return new WaitForSeconds(interval);
        }
    }

    void CreateAfterimage(int index)
    {
        GameObject ai;
        if (index == 1)
            ai = Managers.Resource.Instantiate(afterimagePrefab, transform.position, transform.rotation);
        else
            ai = Managers.Resource.Instantiate(afterimagePrefab, afterimageList[index - 2].transform.position, transform.rotation);
        
        afterimageList.Add(ai);

        if (index == 1)
            ai.GetComponent<SmoothFollower>().Setup(transform, index);
        else
            ai.GetComponent<SmoothFollower>().Setup(afterimageList[index - 2].transform, index);
        
        SpriteRenderer aiSr = ai.GetComponent<SpriteRenderer>();
        aiSr.sprite = spriteRenderer.sprite;
        aiSr.flipX = spriteRenderer.flipX;

        // HSV 랜덤 색상 적용
        float h = UnityEngine.Random.Range(0f, 1f);
        float s = 0.8f;
        float v = 1f;

        Color startColor = Color.HSVToRGB(h, s, v);
        startColor.a = 0.6f / index;
        aiSr.color = startColor;

        // 지속적으로 색이 자연스럽게 바뀌게 만들기
        aiSr.DOKill(); // 기존 트윈 제거
        aiSr.DOColor(GetNextHSVColor(index), 1.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    Color GetNextHSVColor(int index)
    {
        float h = UnityEngine.Random.Range(0f, 1f);
        float s = 0.8f;
        float v = 1f;

        Color nextColor = Color.HSVToRGB(h, s, v);
        nextColor.a = 0.6f / index;
        return nextColor;
    }
    
    private void OnDestroy()
    {
        foreach (GameObject afterimage in afterimageList)
        {
            Managers.Resource.Destroy(afterimage);
        }
    }
}