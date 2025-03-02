using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using Spine.Unity.Examples;

public enum EntityControlType
{
    Player,
    AI
}

public class Entity : MonoBehaviour
{
    #region Events
    public delegate void TakeDamageHandler(Entity entity, Entity instigator, object causer, float damage);
    public delegate void DeadHandler(Entity entity);
    #endregion

    [SerializeField] private Category[] categories;
    [SerializeField] private EntityControlType controlType;

    private Dictionary<string, Transform> socketsByName = new();
    private Rigidbody _rigidbody;
    private Collider _collider;
    public EntityControlType ControlType => controlType;
    public IReadOnlyList<Category> Categories => categories;
    public bool IsPlayer => controlType == EntityControlType.Player;

    public Animator Animator { get; private set; }
    public SkeletonAnimationHandleExample SkeletonAnimator { get; private set; }
    public Stats Stats { get; private set; }
    public bool IsDead => Stats.HPStat != null && Mathf.Approximately(Stats.HPStat.DefaultValue, 0f);
    public Movement Movement { get; private set; }
    public MonoStateMachine<Entity> StateMachine { get; private set; }
    public SkillSystem SkillSystem { get; private set; }
    public Entity Target { get; set; }
    public Rigidbody Rigidbody => _rigidbody;
    

    #region EventHandlers
    public event TakeDamageHandler onTakeDamage;
    public event DeadHandler onDead;
    #endregion

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        SkeletonAnimator = GetComponent<SkeletonAnimationHandleExample>();
        Stats = GetComponent<Stats>();
        Stats.Setup(this);

        Movement = GetComponent<Movement>();
        
        if(controlType == EntityControlType.Player)
            Movement?.Setup(this);
        else
        {
            EntityMovement entityMovement = Movement as EntityMovement;
            entityMovement.Setup(this);
        }

        StateMachine = GetComponent<MonoStateMachine<Entity>>();
        StateMachine?.Setup(this);

        SkillSystem = GetComponent<SkillSystem>();
        SkillSystem?.Setup(this);
        
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }

    private void Start()
    {
        Transform go = transform.Find("BaseAttackCollider");
        if(go)
            go.GetComponent<Attack>().Setup(this, null); // test
    }

    #region DamageHandle
    public void TakeDamage(Entity instigator, object causer, float damage)
    {
        if (IsDead)
            return;

        float prevValue = Stats.HPStat.DefaultValue;
        Stats.HPStat.DefaultValue -= damage;

        onTakeDamage?.Invoke(this, instigator, causer, damage);

        if (Mathf.Approximately(Stats.HPStat.DefaultValue, 0f))
            OnDead();
    }

    private void OnDead()
    {
        if (Movement)
            Movement.enabled = false;

        _rigidbody.isKinematic = true;
        _collider.enabled = false;
        
        SkillSystem.CancelAll(true);
        
        onDead?.Invoke(this);
        SkeletonAnimator.PlayOnlyOneShot("dead", 0);
    }
    #endregion

    #region EntityUtils
    private Transform GetTransformSocket(Transform root, string socketName)
    {
        if (root.name == socketName)
            return root;

        foreach (Transform child in root)
        {
            var socket = GetTransformSocket(child, socketName);
            if (socket)
                return socket;
        }

        return null;
    }

    public Transform GetTransformSocket(string socketName)
    {
        if (socketsByName.TryGetValue(socketName, out var socket))
            return socket;

        // dictionary�� �����Ƿ� ��ȸ �˻�
        socket = GetTransformSocket(transform, socketName);
        if (socket)
            socketsByName[socketName] = socket;

        return socket;
    }

    public bool HasCategory(Category category) => categories.Any(x => x.ID == category.ID);
    #endregion

    #region StateCheck
    public bool IsInState<T>() where T : State<Entity>
        => StateMachine.IsInState<T>();

    public bool IsInState<T>(int layer) where T : State<Entity>
        => StateMachine.IsInState<T>(layer);
    #endregion
}