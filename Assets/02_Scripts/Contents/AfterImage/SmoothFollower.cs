using System;
using UnityEngine;
using DG.Tweening;

public class SmoothFollower : MonoBehaviour
{
    [SerializeField] private float followDuration = 0.5f;  // 따라가는 데 걸리는 시간

    private Tween _moveTween;
    private Transform _target;
    private int _index;
    public void Setup(Transform target, int index)
    {
        this._target = target;
        this._index = index;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, _target.position) > 0.01f)
        {
            _moveTween?.Kill();

            // target 위치로 부드럽게 이동
            _moveTween = transform.DOMove(_target.position, followDuration)
                .SetEase(Ease.OutQuad);
        }
    }

    void OnDestroy()
    {
        _moveTween?.Kill();
    }
}