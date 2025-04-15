using System;
using UnityEngine;
using DG.Tweening;

public class SmoothFollower : MonoBehaviour
{
    public float followDuration = 0.5f;  // 따라가는 데 걸리는 시간

    private Tween moveTween;
    private Transform target;
    private int index;
    public void Setup(Transform target, int index)
    {
        this.target = target;
        this.index = index;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, target.position) > 0.01f)
        {
            moveTween?.Kill();

            // target 위치로 부드럽게 이동
            moveTween = transform.DOMove(target.position, followDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    void OnDestroy()
    {
        moveTween?.Kill();
    }
}