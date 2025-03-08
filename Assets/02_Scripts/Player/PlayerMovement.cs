using System;
using System.Collections;
using Spine.Unity.Examples;
using UnityEngine;
using UnityEngine.InputSystem;
using XNode.Examples.MathNodes;

public enum CharacterState 
{
    None,
    Idle,
    Walk,
    Run,
    Crouch,
    Rise,
    Jump,
    Fall,
    Attack,
    Move,
    Dead
}

public class PlayerMovement : Movement
{
    [SerializeField] private JoyStickManager joystick;
    
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 10f;

    [Header("KeyBoard Or JoyStick")] 
    [SerializeField] private bool isKeyBoard;

    [Header("TakeDamage Setting")] 
    [SerializeField] private float canTakeDamageTime = 1.0f;

    private GameObject _baseAttackCollider;
    private Vector3 _moveInput;
    private Vector3 _velocity;
    private Transform _groundCheck;

    private CharacterState _previousState, _currentState;
    private bool _wasGrounded;
    private bool _isAttacking = false;
    private bool _isJumping = false;
    private bool _isGrounded;

    public override void Setup(Entity entity)
    {
        base.Setup(entity);
        
        _baseAttackCollider = transform.Find("BaseAttackCollider").gameObject;
        _groundCheck = transform.Find("GroundCheck");
        entity.onDead += OnDead;
        entity.onTakeDamage += OnHitDamage;
        animationHandle.PlayAnimationForState("eye blink", 1);
    }

    private void Update()
    {
        //TEST CODE
        if(isKeyBoard)
            KeyBoardUpdate();
        else
            JoystickUpdate();
        
        _isGrounded = Physics.Linecast(transform.position, _groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        MovingCheck();
    }
    private void FixedUpdate()
    {
        if (isCC) return;
        Move();
    }

    public void Attack()
    {
        if (_isAttacking || _isJumping) return;
        StartCoroutine(AttackCo());
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

    private void JoystickUpdate()
    {
        Vector2 joystickInput = joystick.GetInput();
        if (joystickInput.magnitude > 0)
        {
            _moveInput = new Vector3(joystickInput.x, 0, joystickInput.y);
        }
        else if (joystick.IsReleased())
        {
            _moveInput = Vector3.zero;
        }
    }

    private void KeyBoardUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        _moveInput = new Vector3(horizontal, 0, vertical);
    }

    private void MovingCheck()
    {
        if (_isGrounded && !_isAttacking)
        {
            float x = Mathf.Abs(_moveInput.x);
            float z = Mathf.Abs(_moveInput.z);

            if (x > 0 || z > 0)
            {
                _currentState = (x >= 0.6f || z >= 0.6f) ? CharacterState.Run : CharacterState.Walk;
            }
            else
            {
                _currentState = CharacterState.Idle;
            }
        }

        if (_previousState != _currentState)
        {
            HandleStateChanged();
        }
        
        _previousState = _currentState;

        if (_moveInput.x != 0)
        {
            var rotation = transform.rotation;
            rotation.y = Mathf.Abs(transform.rotation.y) * (_moveInput.x > 0 ? 1 : -1);
            transform.rotation = rotation;
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

        // 중력 적용
        if (_isGrounded)
        {
            _isJumping = true;
            _velocity.y += Physics.gravity.y * gravityScale * Time.deltaTime;
        }
        else if (_velocity.y < 0)
        {
            _isJumping = false;
            _velocity.y = -2f; // 땅에 붙도록 설정 (0이면 충돌 문제 발생 가능)
        }

        // 최종 이동 처리 (중력 + 조이스틱 이동)
        Vector3 finalMove = transform.position + new Vector3(moveDirection.x, 0, moveDirection.z);
        rigidBody.MovePosition(finalMove);
    }

    private void HandleStateChanged() 
    {
        string stateName = null;
        switch (_currentState) 
        {
            case CharacterState.Idle:
                stateName = "idle";
                break;
            case CharacterState.Walk:
                stateName = "walk";
                break;
            case CharacterState.Run:
                stateName = "run";
                break;
        }

        animationHandle.PlayAnimationForState(stateName, 0);
    }
    
    private void OnHitDamage(Entity owner, Entity insigator, object causer, float damage)
    {
        var isTackle = causer is bool ? (bool)causer : false;
        if(isTackle)
            StartCoroutine(TakeDamageCo());

        animationHandle.PlayOneShot("eye sad", 1, 2f);
    }

    private IEnumerator TakeDamageCo()
    {
        entity.CanTakeDamage = false;
        
        yield return new WaitForSecondsRealtime(canTakeDamageTime);
        
        entity.CanTakeDamage = true;
    }
    
    private void OnDead(Entity entity)
    {
        Stop();
        Destroy(gameObject, 5.0f);
    }
}
