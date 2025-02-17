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
    Attack
}

public class PlayerMovement : Movement
{
    [SerializeField] private JoyStickManager joystick;
    
    [Header("Jumping")]
    [SerializeField] private float jumpSpeed = 10f;

    private GameObject _baseAttackCollider;
    private Vector3 _moveInput;
    private Vector3 _velocity;
    private Entity _entity;

    private CharacterState _previousState, _currentState;
    private bool _wasGrounded;
    private bool _isAttacking = false;
    private bool _isJumping = false;

    private void Start()
    {
        _entity = GetComponent<Entity>();
        _baseAttackCollider = transform.Find("BaseAttackCollider").gameObject;
    }

    private void Update()
    {
        JoystickUpdate();
        MovingCheck();
    
    }
    private void FixedUpdate()
    {
        Move();
    }

    public void Attack()
    {
        if (_isAttacking || _isJumping) return;
        StartCoroutine(AttackCo());
    }

    public void Jump()
    {
        if (Controller.isGrounded) // 땅에 있을 때만 점프 가능
        {
            _velocity.y = jumpSpeed; // 점프 속도 설정
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

    private void MovingCheck()
    {
        bool isGrounded = Controller.isGrounded;

        if (isGrounded && !_isAttacking)
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
        else if (!isGrounded)
        {
            _currentState = _velocity.y > 0 ? CharacterState.Jump : CharacterState.Fall;
        }

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

        // 중력 적용
        if (!Controller.isGrounded)
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
        Vector3 finalMove = new Vector3(moveDirection.x, _velocity.y * Time.deltaTime, moveDirection.z);
        Controller.Move(finalMove);
    }

    private void HandleStateChanged() 
    {
        string stateName = null;
        switch (_currentState) {
            case CharacterState.Idle:
                stateName = "idle";
                break;
            case CharacterState.Walk:
                stateName = "walk";
                break;
            case CharacterState.Run:
                stateName = "run";
                break;
            case CharacterState.Jump:
                stateName = "jump";
                break;
            case CharacterState.Fall:
                stateName = "fall";
                break;
            case CharacterState.Attack:
                stateName = "attack";
                break;
        }

        animationHandle.PlayAnimationForState(stateName, 0);
    }
}
