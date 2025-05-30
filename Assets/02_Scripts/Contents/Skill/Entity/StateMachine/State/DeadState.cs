using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<Entity>
{
    private PlayerController playerController;
    private EntityMovement movement;
    protected override void Setup()
    {
        playerController = Entity.GetComponent<PlayerController>();
        movement = Entity.GetComponent<EntityMovement>();
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

}
