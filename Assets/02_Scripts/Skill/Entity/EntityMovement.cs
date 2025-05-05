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


    private FollowerEntity aiPath;
    private Transform traceTarget;
    private bool isFindTarget;
    private Vector3 destination;
    private Coroutine traceCoroutine;
    private bool isStiffness;
    
    public bool HasArrived => aiPath.reachedDestination;

    public Entity Owner { get; private set; }
    public float ZAttackOffset => zAttackOffset;

    public bool IsFind
    {
        get => isFindTarget;
        set
        {
            if (isFindTarget != value)
                OnFindTarget?.Invoke(this);
            isFindTarget = value;
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
                traceCoroutine = StartCoroutine(TraceUpdate());
        }
    }

    public Vector3 Destination
    {
        get => destination;
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

        aiPath = Owner.GetComponent<FollowerEntity>();

        aiPath.canSearch = true;
        aiPath.canMove = true;
        aiPath.maxSpeed = runSpeed;
        moveSpeed.onValueChanged += OnMoveSpeedChanged;

        owner.onTakeDamage += TakeDamage;
    }

    private void Start()
    {
        aiPath.updateRotation = false;
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
        if(!isStiffness) return;
        
        isStiffness = true;
        StopMoment();
        owner.Animator.PlayOneShot("damaged", 0, 0, RestartMovement);
        
        if (isStiffness)
        {
            float elapsedTime = 0;
            while (elapsedTime >= stifnessCycle)
            {
                elapsedTime += Time.deltaTime;
            }
            isStiffness = false;
        }
    }

    private void SetDestination(Vector3 destination)
    {
        if (aiPath != null && aiPath.enabled)
        {
            aiPath.updatePosition = true;
            aiPath.canMove = true;
            aiPath.destination = destination;
            this.destination = destination;
            onSetDestination?.Invoke(this, destination);
        }
    }

    public void Stop()
    {
        StopTracing();
        aiPath.canMove = false;
    }

    public void StopTracing()
    {
        aiPath.destination = transform.position;
        aiPath.canMove = false;
        aiPath.updatePosition = false;

        if (traceCoroutine != null)
        {
            StopCoroutine(traceCoroutine);
            traceCoroutine = null;
        }
    }

    public void StopMoment()
    {
        aiPath.isStopped = true;
    }

    public void RestartMovement()
    {
        aiPath.isStopped = false;
    }

    public void LookCheck()
    {
        var rotation = transform.localRotation;
        if (traceTarget)
            rotation.y = (traceTarget.transform.position.x >= transform.position.x ? 0 : 180);
        else
            rotation.y = (aiPath.destination.x >= transform.position.x ? 0 : 180);

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
            if (traceTarget == null || aiPath == null || !aiPath.enabled)
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
        => aiPath.maxSpeed = currentValue;
    
    
    #endregion
}
