using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public JoyStickManager joystick;  // UI 조이스틱 참조

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D _rb;
    private Vector2 moveInput;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Vector2 joystickInput = joystick.GetInput();
        if (joystickInput.magnitude > 0)
        {
            moveInput = joystickInput; // 조이스틱 입력이 있을 때만 업데이트
        }
        else if (joystick.IsReleased()) // 조이스틱에서 손을 뗀 경우
        {
            moveInput = Vector2.zero; // 멈추기
        }
    }

    public void OnMove(InputValue value) // Input System에서 호출
    {
        moveInput = value.Get<Vector2>(); // 키보드 / 컨트롤러 입력
    }

    private void FixedUpdate()
    {
        _rb.linearVelocity = moveInput * moveSpeed; // Rigidbody2D 이동 적용
    }
}