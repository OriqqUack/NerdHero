using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class GainLoot : MonoBehaviour
{
    [Header("Following Settings")]
    [SerializeField] private float curveDuration = 2f;
    [SerializeField] private Stat gainStat;
    [SerializeField] private float gainAmount;

    [Space(10)]
    [Header("Drop Settings")]
    [SerializeField] private float _dropRadius = 1f;
    [SerializeField] private float _forwardForce = 1f;
    [SerializeField] private float _upwardForce = 1f;
    [SerializeField] private bool _isEnergy;
    
    private bool _isAttracted = false;
    private Vector3 _startPosition;
    private float t = 0f;
    private Transform _player;
    private Entity _playerEntity;
    private Rigidbody _rb;
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerEntity = _player.GetComponent<Entity>();

        if (!_isEnergy)
            Floating();
    }

    private void Floating()
    {
        _rb = GetComponent<Rigidbody>();
        // �ֺ� �� �ȿ��� ������ ��ġ�� ����
        Vector2 randomCircle = Random.insideUnitCircle * _dropRadius;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y);

        // �������� �ڿ� �ֺ� ���� ���� ���� �̵�
        transform.position += randomPosition;

        // �ϴ÷� ��¦ �������� ��
        Vector3 randomDirection = (transform.forward + transform.up).normalized;
        _rb.AddForce(randomDirection * _forwardForce, ForceMode.Impulse);  // ������ �ణ ������ ��
        _rb.AddForce(Vector3.up * _upwardForce, ForceMode.Impulse);        // �ϴ÷� �ߴ� ��

        // ���� ȸ�� ȿ�� �߰�
        _rb.AddTorque(new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)), ForceMode.Impulse);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isAttracted)
        {
            _isAttracted = true;
            _startPosition = transform.position;
            _player = other.transform;
            MoveToPlayer();
        }
    }

    private void MoveToPlayer()
    {
        Vector3 midPoint = (_startPosition + _player.position) / 2 + Vector3.up * 2f; // 중간 지점을 위로 이동

        // Bezier 곡선 따라 이동하는 애니메이션 적용
        DOTween.To(() => t, x => t = x, 1f, curveDuration)
            .OnUpdate(() => transform.position = CalculateBezierCurve(_startPosition, midPoint, _player.position, t))
            .OnComplete(() => Gained());
    }

    private Vector3 CalculateBezierCurve(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        Vector3 p0 = Vector3.Lerp(start, control, t);
        Vector3 p1 = Vector3.Lerp(control, end, t);
        return Vector3.Lerp(p0, p1, t);
    }

    private void Gained()
    {
        _playerEntity.Stats.IncreaseDefaultValue(gainStat, gainAmount);
        Destroy(gameObject);
    }
}