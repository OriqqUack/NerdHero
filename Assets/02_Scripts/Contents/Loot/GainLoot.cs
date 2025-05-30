using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GainType { ExpAndEnergy }

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
    
    private bool _isAttracted = false;
    private float _t = 0f;
    private Vector3 _startPosition;
    private Transform _player;
    private Transform _endPoint;
    private Entity _playerEntity;
    private Rigidbody _rb;
    private ItemSO _gainItem; // 획득할 아이템 데이터
    private GainType _gainType; // 스탯 or 아이템 획득 타입 설정

    private Tween _moveTween;
    private Stat[] _gainStat;
    private float[] _gainAmount;
    private float _attractDistanceSqr;
    private float _moveDuration = 0.5f; // 이동 총 시간
    private bool _isSetup;
    private Entity _entity;
    
    public void Setup(Entity owner, DropTable.DropEntry entry)
    {
        _entity = owner;
        _player = WaveManager.Instance.PlayerTransform;
        _playerEntity = _player.GetComponent<Entity>();

        _gainType = entry.gainType;
        _gainStat = entry.stat;
        _gainAmount = entry.statAmount;
        
        if (isFloating)
            Floating();
        
        _attractDistanceSqr = attractDistance * attractDistance;
        
        _isSetup = true;
    }

    private void Floating()
    {
        _rb = GetComponent<Rigidbody>();
        Vector2 randomCircle = Random.insideUnitCircle * dropRadius;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y);

        transform.position += randomPosition;

        Vector3 randomDirection = (transform.forward + transform.up).normalized;
        _rb.AddForce(randomDirection * forwardForce, ForceMode.Impulse);
        _rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

        _rb.AddTorque(new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)), ForceMode.Impulse);
    }

    private void Update()
    {
        if (!_isSetup || _player == null)
            return;

        if (!_isAttracted)
        {
            if (Vector3.SqrMagnitude(transform.position - _player.position) <= _attractDistanceSqr)
            {
                if (_rb != null)
                {
                    _rb.isKinematic = true;
                }
                _startPosition = transform.position;
                _attractDistanceSqr = 0f;
                _isAttracted = true;
            }
        }
        else
        {
            _attractDistanceSqr += Time.deltaTime / _moveDuration; // t는 0 → 1로 증가 (1초 걸리게)
            _attractDistanceSqr = Mathf.Clamp01(_attractDistanceSqr);

            // 매 프레임 현재 플레이어 위치 반영
            Vector3 targetPosition = _player.position + Vector3.up * 1f;
            Vector3 midPoint = (_startPosition + targetPosition) / 2f + Vector3.up * 2f; // 가운데를 위로 띄워서 부드럽게

            transform.position = CalculateBezierCurve(_startPosition, midPoint, targetPosition, _attractDistanceSqr);

            if (_attractDistanceSqr >= 1f)
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
        switch (_gainType)
        {
            case GainType.ExpAndEnergy:
                if (statFactor)
                {
                    float factor = _playerEntity.Stats.GetValue(statFactor);
                    Debug.Log(factor);
                    _gainAmount[1] *= (1 + factor);
                }
                _playerEntity.Stats.IncreaseDefaultValue(_gainStat[0], _entity.Stats.ExpCharge.Value);
                _playerEntity.Stats.IncreaseDefaultValue(_gainStat[1], _gainAmount[1]);
                break;
        }

        Destroy(gameObject);
    }
}
