using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AfterimageGenerator : MonoBehaviour
{
    public GameObject afterimagePrefab; // 잔상 프리팹 (SpriteRenderer 포함)
    public float interval = 0.1f;       // 잔상 생성 간격

    private List<GameObject> afterimageList = new List<GameObject>();
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(GenerateAfterimage());
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
        if(index == 1)
            ai = Managers.Resource.Instantiate(afterimagePrefab, transform.position, transform.rotation);
        else
            ai = Managers.Resource.Instantiate(afterimagePrefab, afterimageList[index - 2].transform.position, transform.rotation);

        afterimageList.Add(ai);
        if(index == 1)
            ai.GetComponent<SmoothFollower>().Setup(transform, index);
        else
            ai.GetComponent<SmoothFollower>().Setup(afterimageList[index - 2].transform, index);

        SpriteRenderer aiSr = ai.GetComponent<SpriteRenderer>();
        aiSr.sprite = spriteRenderer.sprite;
        aiSr.flipX = spriteRenderer.flipX;
        aiSr.color = new Color(1f, 1f, 1f, 0.6f / index); // 초기 투명도 설정
    }

    private void OnDestroy()
    {
        foreach (GameObject afterimage in afterimageList)
        {
            Managers.Resource.Destroy(afterimage);
        }
    }
}