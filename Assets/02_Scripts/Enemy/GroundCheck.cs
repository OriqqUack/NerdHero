using System;
using UnityEngine;
using Pathfinding;

public class GroundCheck : MonoBehaviour
{
    public bool IsOnGround;
    private FollowerEntity aiPath;
    private Rigidbody rb;

    private void Awake()
    {
        aiPath = GetComponent<FollowerEntity>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsOnGround = true;
            aiPath.canMove = true;
            aiPath.updatePosition = true;
            aiPath.Teleport(transform.position);

            Debug.Log("Check Ground - A*");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            IsOnGround = false;
            aiPath.canMove = false;

            Debug.Log("Left Ground - A*");
        }
    }
}