using UnityEngine;

public class ProjectileBase : Projectile
{
    private void FixedUpdate()
    {
        rigidBody.linearVelocity = direction * speed;
    }
}
