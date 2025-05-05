using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GainType { Exp, Energy, Item, Heart }

public class GainLoot : MonoBehaviour
{
    [Space(10)]
    [Header("Drop Settings")]
    [SerializeField] private float dropRadius = 1f;
    [SerializeField] private float forwardForce = 1f;
    [SerializeField] private float upwardForce = 1f;
    [SerializeField] private bool isFloating;
    [SerializeField] private float attractDistance = 2f; // 플레이어가 접근하면 자동 획득하는 거리
    [SerializeField] private Stat statFactor;
    
    private bool isAttracted = false;
    private Vector3 startPosition;
    private float t = 0f;
    private Transform player;
    private Transform endPoint;
    private Entity playerEntity;
    private Rigidbody rb;
    private GainType gainType; // 스탯 or 아이템 획득 타입 설정
    private ItemSO gainItem; // 획득할 아이템 데이터

    private Tween moveTween;
    private Stat gainStat;
    private float gainAmount;
    private float attractDistanceSqr;
    private float moveDuration = 0.5f; // 이동 총 시간
    private bool isSetup;
    private Entity _entity;
    
    public void Setup(Entity owner, DropTable.DropEntry entry)
    {
        _entity = owner;
        player = WaveManager.Instance.PlayerTransform;
        playerEntity = player.GetComponent<Entity>();

        gainType = entry.gainType;
        gainStat = entry.stat;
        gainAmount = entry.statAmount;
        if(entry.item)
            gainItem = entry.item;
        
        if (isFloating)
            Floating();
        
        attractDistanceSqr = attractDistance * attractDistance;
        
        isSetup = true;
    }

    private void Floating()
    {
        rb = GetComponent<Rigidbody>();
        Vector2 randomCircle = Random.insideUnitCircle * dropRadius;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y);

        transform.position += randomPosition;

        Vector3 randomDirection = (transform.forward + transform.up).normalized;
        rb.AddForce(randomDirection * forwardForce, ForceMode.Impulse);
        rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        rb.AddTorque(new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)), ForceMode.Impulse);
    }

    private void Update()
    {
        if (!isSetup || player == null)
            return;

        if (!isAttracted)
        {
            if (Vector3.SqrMagnitude(transform.position - player.position) <= attractDistanceSqr)
            {
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
                startPosition = transform.position;
                t = 0f;
                isAttracted = true;
            }
        }
        else
        {
            t += Time.deltaTime / moveDuration; // t는 0 → 1로 증가 (1초 걸리게)
            t = Mathf.Clamp01(t);

            // 매 프레임 현재 플레이어 위치 반영
            Vector3 targetPosition = player.position + Vector3.up * 1f;
            Vector3 midPoint = (startPosition + targetPosition) / 2f + Vector3.up * 2f; // 가운데를 위로 띄워서 부드럽게

            transform.position = CalculateBezierCurve(startPosition, midPoint, targetPosition, t);

            if (t >= 1f)
            {
                Gained();
            }
        }
    }

    private Vector3 CalculateBezierCurve(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        Vector3 p0 = Vector3.Lerp(start, control, t);
        Vector3 p1 = Vector3.Lerp(control, end, t);
        return Vector3.Lerp(p0, p1, t);
    }

    private void Gained()
    {
        switch (gainType)
        {
            case GainType.Exp:
                playerEntity.Stats.IncreaseDefaultValue(gainStat, _entity.Stats.ExpCharge.Value);
                break;
            case GainType.Energy:
                if (statFactor)
                {
                    float factor = playerEntity.Stats.GetValue(statFactor);
                    Debug.Log(factor);
                    gainAmount *= (1 + factor);
                }
                playerEntity.Stats.IncreaseDefaultValue(gainStat, gainAmount);
                break;
            case GainType.Item:
                if (!gainItem) break;
                WaveManager.Instance.AddGainedItem(gainItem);
                break;
            case GainType.Heart:
                playerEntity.Stats.IncreaseDefaultValue(gainStat, gainAmount);
                break;
        }

        Destroy(gameObject);
    }
}
