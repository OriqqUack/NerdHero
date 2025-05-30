using UnityEngine;

public class ProjectileBase : Projectile
{
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        rigidBody.linearVelocity = direction * speed;
    }
}
