using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : Movement
{
    #region Property
    #region Events
    public delegate void SetDestinationHandler(EntityMovement movement, Vector3 destination);
    public delegate void FindTargetHandler(EntityMovement movement);
    #endregion
    [SerializeField] private float zAttackOffset;
    private NavMeshAgent agent;
    private Transform traceTarget;
    private bool isFindTarget;
    
    public bool HasArrived =>
        !agent.pathPending && 
        agent.remainingDistance <= agent.stoppingDistance && 
        agent.velocity.sqrMagnitude < 0.01f;
    
    public Entity Owner { get; private set; }
    public bool IsRolling { get; private set; }
    public float ZAttackOffset => zAttackOffset;

    public bool IsFind
    {
        get => isFindTarget;
        set
        { 
            if(isFindTarget != value)
                OnFindTarget?.Invoke(this);
            isFindTarget = value;
        }
    }

    public Transform TraceTarget
    {
        get => traceTarget;
        set
        {
            /*if (traceTarget == value)
                return;*/

            ForceStop();

            traceTarget = value;
            onSetDestination?.Invoke(this, traceTarget.transform.position);

            if (traceTarget)
                StartCoroutine("TraceUpdate");
        }
    }

    public Vector3 Destination
    {
        get => agent.destination;
        set
        {
            SetDestination(value);
        }
    }

    public event SetDestinationHandler onSetDestination;
    public event FindTargetHandler OnFindTarget;
    #endregion

    #region Methods
    public override void Setup(Entity owner)
    {
        base.Setup(owner);
        Owner = owner;

        agent = Owner.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;

        agent.speed = runSpeed;
        moveSpeed.onValueChanged += OnMoveSpeedChanged;
    }

    private void OnDisable() => Stop();

    private void OnDestroy()
    {
        if (moveSpeed)
            moveSpeed.onValueChanged -= OnMoveSpeedChanged;
    }

    private void SetDestination(Vector3 destination)
    {
        agent.destination = destination;
        LookCheck();
        //LookAt(destination);
    }

    public void Stop()
    {
        traceTarget = null;
        StopCoroutine("TraceUpdate");

        if (agent.isOnNavMesh)
            agent.ResetPath();

        agent.velocity = Vector3.zero;
    }

    public void ForceStop()
    {
        StopCoroutine("TraceUpdate");

        if (agent.isOnNavMesh)
            agent.ResetPath();

        agent.velocity = Vector3.zero;
    }
    #endregion

    #region LookMethod

    public void LookCheck()
    {
        var rotation = transform.rotation;
        if(traceTarget)
            rotation.y = Mathf.Abs(transform.rotation.y) * (traceTarget.transform.position.x >= transform.position.x ? 1 : -1);
        else
            rotation.y = Mathf.Abs(transform.rotation.y) * (agent.destination.x >= transform.position.x ? 1 : -1);
        transform.rotation = rotation;
    }
    /*public void LookAt(Vector3 position)
    {
        StopCoroutine("LookAtUpdate");
        StartCoroutine("LookAtUpdate", position);
    }

    public void LookAtImmediate(Vector3 position)
    {
        position.y = transform.position.y;
        var lookDirection = (position - transform.position).normalized;
        var rotation = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : transform.rotation;
        transform.rotation = rotation;
    }

    private IEnumerator LookAtUpdate(Vector3 position)
    {
        position.y = transform.position.y;
        var lookDirection = (position - transform.position).normalized;
        var rotation = lookDirection != Vector3.zero ? Quaternion.LookRotation(lookDirection) : transform.rotation;
        var speed = 180f / 0.15f;

        while (true)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, speed * Time.deltaTime);
            if (transform.rotation == rotation)
                break;

            yield return null;
        }
    }*/
    #endregion

    #region Tracing Method
    private IEnumerator TraceUpdate()
    {
        float attackRange = Owner.Stats.GetStat("ATTACK_RANGE").Value;
        float stopDistance = 0.3f; // 멈추는 거리
        while (true)
        {
            var pos = TraceTarget.position;
            // 대상이 공격 범위 밖에 있는 경우, attackRange 만큼 조정
            if (Vector3.SqrMagnitude(TraceTarget.position - transform.position) > attackRange * attackRange)
            {
                if (TraceTarget.position.x < transform.position.x)
                    pos.x = TraceTarget.position.x + attackRange;
                else
                    pos.x = TraceTarget.position.x - attackRange;
            }
            else
            {
                pos.x = transform.position.x;
            }
            
            float randomZ = Random.Range(-ZAttackOffset, ZAttackOffset);
            pos.z += randomZ;

            SetDestination(pos); // 최종 위치 설정

            // 목표 지점에 도달하면 탈출
            if (Vector3.SqrMagnitude(transform.position - pos) <= stopDistance * stopDistance)
            {
                SetDestination(transform.position); // 최종 위치 설정
                break;
            }
            yield return null;
        }
    }


    private void OnMoveSpeedChanged(Stat stat, float currentValue, float prevValue)
        => agent.speed = currentValue;
    #endregion
}
