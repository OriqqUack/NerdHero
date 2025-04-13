using System.Collections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[System.Serializable]
public class SpawnSmallObjectAction : CustomAction
{
    [Header("SpawnPrefab Settings")]
    [SerializeField] private GameObject smallObjectPrefab;
    [SerializeField] private int count;
    
    [Space(10)] [Header("Drop Settings")]
    [SerializeField] private float _dropRadius = 1f;
    [SerializeField] private float _forwardForce = 1f;
    [SerializeField] private float _upwardForce = 1f;

    private Entity _entity;
    public override void Run(object data)
    {
        var skillData = data as Skill;
        if (skillData == null || smallObjectPrefab == null || count <= 0) return;

        float angleStep = 360f / count; // 개수에 따라 균등한 각도 계산

        for (int i = 0; i < count; i++)
        {
            // 각도 계산
            float angle = angleStep * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));

            // 객체 생성 및 초기화
            GameObject go = GameObject.Instantiate(smallObjectPrefab, skillData.Owner.transform.position, Quaternion.identity);
            go.transform.localEulerAngles = new Vector3(0, 90, 0);
            _entity = go.transform.GetComponentInChildren<Entity>();
            _entity.GetComponent<NavMeshAgent>().updatePosition = false;
            // 랜덤한 위치 이동 적용
            Floating(direction);
        }
    }
    
    private void Floating(Vector3 direction)
    {
        var agent = _entity.GetComponent<NavMeshAgent>();
        var rigid = _entity.Rigidbody;

        agent.updatePosition = false;

        // 힘 적용 (지정된 방향 + 위쪽)
        Vector3 forceDirection = (direction + Vector3.up).normalized;
        rigid.AddForce(forceDirection * _upwardForce, ForceMode.Impulse);

        // 감지 루프 시작 (DOTween으로 velocity.magnitude 감시)
        DOTween.To(() => rigid.linearVelocity.magnitude, x => { }, 0f, 2f) // 2초 이내에 멈출 거라고 가정
            .SetEase(Ease.Linear)
            .OnUpdate(() =>
            {
                if (rigid.linearVelocity.magnitude <= 0.1f)
                {
                    agent.updatePosition = true;
                    rigid.linearVelocity = Vector3.zero; // 혹시 남아있는 미세한 움직임 제거
                }
            })
            .OnComplete(() =>
            {
                // 안전장치: 도중에 안 멈춰도 2초 후 강제로 설정
                if (!agent.updatePosition)
                {
                    agent.updatePosition = true;
                    rigid.linearVelocity = Vector3.zero;
                }
            });
    }

    public override object Clone() => new SpawnSmallObjectAction();
}
