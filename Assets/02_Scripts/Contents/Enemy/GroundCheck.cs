using System;
using UnityEngine;
using Pathfinding;

public class GroundCheck : MonoBehaviour
{
    public bool IsOnGround;
    private FollowerEntity _aiPath;

    private void Awake()
    {
        _aiPath = GetComponent<FollowerEntity>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsOnGround = true;
            _aiPath.canMove = true;
            _aiPath.updatePosition = true;
            _aiPath.Teleport(transform.position);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsOnGround = false;
            _aiPath.canMove = false;

        }
    }
}