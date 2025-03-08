using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GroundCheck : MonoBehaviour
{
    public bool IsOnGround;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            agent.nextPosition = transform.position;
            agent.updatePosition = true;
            IsOnGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            agent.updatePosition = false;
            IsOnGround = false;
        }
    }
}
