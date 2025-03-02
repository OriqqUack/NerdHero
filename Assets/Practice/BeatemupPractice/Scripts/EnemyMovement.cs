using System.Collections;
using UnityEngine;

public class EnemyMovement : Movement
{
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 10f;

    [SerializeField] private GameObject dropItem;
    
    private float _attackSpeed;
    private GameObject _baseAttackCollider;
    private Vector3 _moveInput;
    private Vector3 _velocity;
    private Transform _groundCheck;
    private Entity _entity;
    
    private CharacterState _previousState, _currentState;
    private bool _wasGrounded;
    private bool _isAttacking = false;
    private bool _isJumping = false;
    private bool _isGrounded;
    private float _zForce;
    private float _walkTimer;
    private float _nextAttack;
    
    public bool HasArrived { get; set; }
    
    public override void Setup(Entity entity)
    {
        base.Setup(entity);
        
        _baseAttackCollider = transform.Find("BaseAttackCollider").gameObject;
        _groundCheck = transform.Find("GroundCheck");
        _attackSpeed = entity.Stats.GetStat("ATTACK_SPEED").Value;
        TraceTarget = GameObject.FindGameObjectWithTag("Player").transform;
        entity.onDead += OnDead;
    }

    private void Update()
    {
        _isGrounded = Physics.Linecast(transform.position, _groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        _walkTimer += Time.deltaTime;
        MoveEnemyUpdate();
        MovingCheck();
    }
    private void FixedUpdate()
    {
        if (isCC) return;
        Move();
        AttackCheck();
    }

    public void AttackCheck()
    {
        Vector3 targetDistance = TraceTarget.position - transform.position;
        if (Mathf.Abs(targetDistance.x) < 1.5f && Mathf.Abs(targetDistance.z) < 1.5f && Time.time > _nextAttack)
        {
            _currentState = CharacterState.Attack;
            StartCoroutine(AttackCo());
            _nextAttack = Time.time + (1 - _attackSpeed);
        }
    }

    public void Jump()
    {
        if (_isGrounded) // 땅에 있을 때만 점프 가능
        {
            rigidBody.AddForce(_moveInput * jumpSpeed, ForceMode.Impulse); // 점프 속도 설정
            _currentState = CharacterState.Jump;
        }
    }

    private IEnumerator AttackCo()
    {
        _isAttacking = true;
        _currentState = CharacterState.Attack;
        _baseAttackCollider.SetActive(true);
        yield return new WaitForSeconds(animationHandle.GetAnimationForState("attack").Duration);
        
        _baseAttackCollider.SetActive(false);
        _isAttacking = false;
    }
    
    private void MoveEnemyUpdate()
    {
        Vector3 targetDistance = TraceTarget.position - transform.position;
        float hForce = targetDistance.x != 0 ? Mathf.Sign(targetDistance.x) : 0;

        if (_walkTimer >= Random.Range(1f, 2f))
        {
            _zForce = Random.Range(-1, 2);
            _walkTimer = 0;
        }

        if (Mathf.Abs(targetDistance.x) < 1.5f)
        {
            hForce = 0;
        }

        _moveInput = new Vector3(hForce * runSpeed, 0, _zForce * runSpeed);
    }

    private void MovingCheck()
    {
        if (_isGrounded && !_isAttacking)
        {
            float x = Mathf.Abs(_moveInput.x);
            float z = Mathf.Abs(_moveInput.z);

            if (x > 0 || z > 0)
            {
                _currentState = CharacterState.Move;
            }
            else
            {
                _currentState = CharacterState.Idle;
            }
        }
        /*else if (!_isGrounded)
        {
            _currentState = _velocity.y > 0 ? CharacterState.Jump : CharacterState.Fall;
        }*/

        if (_previousState != _currentState)
        {
            HandleStateChanged();
        }

        _previousState = _currentState;

        if (_moveInput.x != 0)
        {
            var scale = transform.localScale;
            scale.x = _moveInput.x > 0 ? 1 : -1;
            transform.localScale = scale;
        }
    }

    private void Move()
    {
        if (_isAttacking) return;
        
        if (_moveInput.magnitude > 1) 
        {
            _moveInput.Normalize(); // 대각선 속도 보정
        }

        float speed = (Mathf.Abs(_moveInput.x) >= 0.6f || Mathf.Abs(_moveInput.z) >= 0.6f) ? runSpeed : walkSpeed;
        Vector3 moveDirection = _moveInput * (speed * Time.deltaTime);

        // 최종 이동 처리 (중력 + 조이스틱 이동)
        Vector3 finalMove = transform.position + new Vector3(moveDirection.x, 0, moveDirection.z);
        rigidBody.MovePosition(finalMove);
    }

    private void HandleStateChanged() 
    {
        string stateName = null;
        switch (_currentState) {
            case CharacterState.Idle:
                stateName = "idle";
                break;
            case CharacterState.Move:
                stateName = "move";
                break;
            case CharacterState.Attack:
                stateName = "attack";
                break;
            case CharacterState.Dead:
                stateName = "dead";
                break;
        }

        animationHandle.PlayAnimationForState(stateName, 0);
    }

    private void OnDead(Entity entity)
    {
        Stop();
        Instantiate(dropItem, transform.position, Quaternion.identity);
        Destroy(gameObject, 5.0f);
    }
}
