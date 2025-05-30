using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    public Vector3 MoveDirection;
    
    public void OnMove(InputValue value) => MoveDirection = value.Get<Vector3>();
}
