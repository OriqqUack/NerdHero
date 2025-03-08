using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[System.Serializable]
public class SpawnSmallObjectAction : CustomAction
{
    [Header("SpawnPrefab Settings")]
    [SerializeField] private Entity smallObjectPrefab;
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
            _entity = GameObject.Instantiate(smallObjectPrefab, skillData.Owner.transform.position, Quaternion.identity);
            _entity.transform.localEulerAngles = new Vector3(0, 90, 0);
            // 랜덤한 위치 이동 적용
            Floating(direction);
        }
    }
    
    private void Floating(Vector3 direction)
    {
        /*// 랜덤 원형 범위 내 위치
        Vector2 randomCircle = Random.insideUnitCircle * _dropRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0, randomCircle.y);
        entity.transform.position += randomOffset;*/
        _entity.GetComponent<NavMeshAgent>().updatePosition = false;
        
        // 힘 적용 (지정된 방향 + 위쪽)
        Vector3 forceDirection = (direction + Vector3.up).normalized;
        _entity.Rigidbody.AddForce(forceDirection * _upwardForce, ForceMode.Impulse);  
        
        //entity.GetComponent<GroundCheck>().GroundCheckStart();

        /*// 랜덤 회전 추가
        entity.Rigidbody.AddTorque(new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)), ForceMode.Impulse);*/
    }

    


    
    public override object Clone() => new SpawnSmallObjectAction();
}
