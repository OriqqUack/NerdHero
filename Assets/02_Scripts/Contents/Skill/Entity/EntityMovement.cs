using System;
using System.Collections;
using UnityEngine;
using Pathfinding;
using Random = UnityEngine.Random;

public class EntityMovement : Movement
{
    #region Property
    public delegate void SetDestinationHandler(EntityMovement movement, Vector3 destination);
    public delegate void FindTargetHandler(EntityMovement movement);

    [SerializeField] private float zAttackOffset;
    [SerializeField] private float stifnessCycle = 3.0f;

    private FollowerEntity _aiPath;
    private Transform _traceTarget;
    private bool _isFindTarget;
    private Vector3 _destination;
    private Coroutine _traceCoroutine;
    private bool _isStiffness;
    
    public bool HasArrived => _aiPath.reachedDestination;

    public Entity Owner { get; private set; }
    public float ZAttackOffset => zAttackOffset;

    public bool IsFind
    {
        get => _isFindTarget;
        set
        {
            if (_isFindTarget != value)
                OnFindTarget?.Invoke(this);
            _isFindTarget = value;
        }
    }

    public Transform TraceTarget
    {
        get => traceTarget;
        set
        {
            StopTracing();
            traceTarget = value;

            if (traceTarget != null)
                _traceCoroutine = StartCoroutine(TraceUpdate());
        }
    }

    public Vector3 Destination
    {
        get => _destination;
        set => SetDestination(value);
    }

    public event SetDestinationHandler onSetDestination;
    public event FindTargetHandler OnFindTarget;
    #endregion

    #region Methods
    public override void Setup(Entity owner)
    {
        base.Setup(owner);
        Owner = owner;

        _aiPath = Owner.GetComponent<FollowerEntity>();

        _aiPath.canSearch = true;
        _aiPath.canMove = true;
        _aiPath.maxSpeed = runSpeed;
        moveSpeed.onValueChanged += OnMoveSpeedChanged;

        owner.onTakeDamage += TakeDamage;
    }

    private void Start()
    {
        _aiPath.updateRotation = false;
    }

    private void Update()
    {
        
    }

    private void OnDisable() => Stop();

    private void OnDestroy()
    {
        if (moveSpeed)
            moveSpeed.onValueChanged -= OnMoveSpeedChanged;
    }

    private void TakeDamage(Entity instigator, Entity owner, object causer, float damage)
    {
        if(!_isStiffness) return;
        
        _isStiffness = true;
        StopMoment();
        owner.Animator.PlayOneShot("damaged", 0, 0, RestartMovement);
        
        if (_isStiffness)
        {
            float elapsedTime = 0;
            while (elapsedTime >= stifnessCycle)
            {
                elapsedTime += Time.deltaTime;
            }
            _isStiffness = false;
        }
    }

    private void SetDestination(Vector3 destination)
    {
        if (_aiPath != null && _aiPath.enabled)
        {
            _aiPath.updatePosition = true;
            _aiPath.canMove = true;
            _aiPath.destination = destination;
            this._destination = destination;
            onSetDestination?.Invoke(this, destination);
        }
    }

    public void Stop()
    {
        StopTracing();
        _aiPath.canMove = false;
    }

    public void StopTracing()
    {
        _aiPath.destination = transform.position;
        _aiPath.canMove = false;
        _aiPath.updatePosition = false;

        if (_traceCoroutine != null)
        {
            StopCoroutine(_traceCoroutine);
            _traceCoroutine = null;
        }
    }

    public void StopMoment()
    {
        _aiPath.isStopped = true;
    }

    public void RestartMovement()
    {
        _aiPath.isStopped = false;
    }

    public void LookCheck()
    {
        var rotation = transform.localRotation;
        if (traceTarget)
            rotation.y = (traceTarget.transform.position.x >= transform.position.x ? 0 : 180);
        else
            rotation.y = (_aiPath.destination.x >= transform.position.x ? 0 : 180);

        transform.localRotation = rotation;
    }

    public override void Clear()
    {
        StopTracing();
    }

    #endregion

    #region Trace Logic
    private IEnumerator TraceUpdate()
    {
        float attackRange = Owner.Stats.GetStat("ATTACK_RANGE").Value;
        float stopDistance = 0.3f;

        while (true)
        {
            if (traceTarget == null || _aiPath == null || !_aiPath.enabled)
                yield break;

            Vector3 pos = traceTarget.position;

            if (Vector3.SqrMagnitude(traceTarget.position - transform.position) > attackRange * attackRange)
            {
                pos.x += (traceTarget.position.x < transform.position.x) ? attackRange : -attackRange;
            }
            else
            {
                pos.x = transform.position.x;
            }

            pos.z += Random.Range(-ZAttackOffset, ZAttackOffset);

            SetDestination(pos);

            if (Vector3.SqrMagnitude(transform.position - pos) <= stopDistance * stopDistance)
            {
                SetDestination(transform.position);
                break;
            }

            yield return null;
        }
    }

    private void OnMoveSpeedChanged(Stat stat, float currentValue, float prevValue)
        => _aiPath.maxSpeed = currentValue;
    
    
    #endregion
}
