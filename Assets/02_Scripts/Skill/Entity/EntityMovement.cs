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

    
    private NavMeshAgent agent;
    private Transform traceTarget;
    private bool isFindTarget;
    
    public bool HasArrived =>
        !agent.pathPending && 
        agent.remainingDistance <= agent.stoppingDistance && 
        agent.velocity.sqrMagnitude < 0.01f;
    
    public Entity Owner { get; private set; }
    public bool IsRolling { get; private set; }

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

            Stop();

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
            TraceTarget = null;
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
        while (true)
        {
            if (Vector3.SqrMagnitude(TraceTarget.position - transform.position) > 1.0f)
            {
                SetDestination(TraceTarget.position);
                yield return null;
            }
            else
                break;
        }
    }

    private void OnMoveSpeedChanged(Stat stat, float currentValue, float prevValue)
        => agent.speed = currentValue;
    #endregion
}
