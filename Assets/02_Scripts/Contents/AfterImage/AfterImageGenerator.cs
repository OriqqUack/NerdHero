using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

//잔상이 따라가는 스크립트
public class AfterimageGenerator : MonoBehaviour
{
    public GameObject afterimagePrefab; // 잔상 프리팹 (SpriteRenderer 포함)
    public GameObject blinkImagePrefab;
    public float interval = 0.1f;       // 잔상 생성 간격

    private List<GameObject> _afterimageList = new List<GameObject>();
    private SpriteRenderer _spriteRenderer;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            ai = Managers.Resource.Instantiate(afterimagePrefab, _afterimageList[index - 2].transform.position, transform.rotation);
        
        _afterimageList.Add(ai);

        if (index == 1)
            ai.GetComponent<SmoothFollower>().Setup(transform, index);
        else
            ai.GetComponent<SmoothFollower>().Setup(_afterimageList[index - 2].transform, index);
        
        SpriteRenderer aiSr = ai.GetComponent<SpriteRenderer>();
        aiSr.sprite = _spriteRenderer.sprite;
        aiSr.flipX = _spriteRenderer.flipX;

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
    // 모든 DOTween 애니메이션 중지
    DOTween.KillAll();
    
    // 씬이 종료 중인지 확인
    if (Managers.IsDestroying) return;
    
    foreach (GameObject afterimage in _afterimageList)
    {
        if (afterimage != null)
        {
            Managers.Resource.Destroy(afterimage);
        }
    }
    _afterimageList.Clear();
}
}