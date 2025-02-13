using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public Vector2 MoveDirection;
    
    public void OnMove(InputValue value) => MoveDirection = value.Get<Vector2>();
}
