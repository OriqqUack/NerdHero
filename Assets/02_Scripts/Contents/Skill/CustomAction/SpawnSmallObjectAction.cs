using System.Collections;
using DG.Tweening;
using UnityEngine;
using Pathfinding;

[System.Serializable]
public class SpawnSmallObjectAction : CustomAction
{
    [Header("SpawnPrefab Settings")]
    [SerializeField] private GameObject smallObjectPrefab;
    [SerializeField] private int count;
    [SerializeField] private AudioClip soundEffect;

    [Space(10)] [Header("Drop Settings")]
    [SerializeField] private float _dropRadius = 1f;
    [SerializeField] private float _forwardForce = 1f;
    [SerializeField] private float _upwardForce = 1f;

    private Entity _entity;

    public override void Run(object data)
    {
        var skillData = data as Skill;
        if (skillData == null || smallObjectPrefab == null || count <= 0) return;

        float angleStep = 360f / count;
        
        if(soundEffect)
            Managers.SoundManager.Play(soundEffect);

        for (int i = 0; i < count; i++)
        {
            float angle = angleStep * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject go = GameObject.Instantiate(smallObjectPrefab, skillData.Owner.transform.position, Quaternion.identity);
            go.transform.localEulerAngles = new Vector3(0, 90, 0);

            _entity = go.transform.GetComponentInChildren<Entity>();
            WaveManager.Instance.AddEnemies(_entity);
            var aiPath = _entity.GetComponent<FollowerEntity>();
            if (aiPath != null)
            {
                aiPath.canMove = false;
                aiPath.updatePosition = false;
            }

            Floating(direction, aiPath);
        }
    }

    private void Floating(Vector3 direction, FollowerEntity aiPath)
    {
        var rigid = _entity.Rigidbody;

        // 힘 적용
        Vector3 forceDirection = (direction + Vector3.up).normalized;
        rigid.AddForce(forceDirection * _upwardForce, ForceMode.Impulse);

        // 감시 루프
        DOTween.To(() => rigid.linearVelocity.magnitude, x => { }, 0f, 2f)
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (rigid.linearVelocity.magnitude <= 0.1f)
                {
                    if (aiPath != null)
                    {
                        aiPath.updatePosition = true;
                        aiPath.canMove = true;
                    }
                    rigid.linearVelocity = Vector3.zero;
                }
            })
            .OnComplete(() =>
            {
                if (aiPath != null && !aiPath.updatePosition)
                {
                    aiPath.updatePosition = true;
                    aiPath.canMove = true;
                    rigid.linearVelocity = Vector3.zero;
                }
            });
    }

    public override object Clone() => new SpawnSmallObjectAction();
}
