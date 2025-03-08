using UnityEngine;

public class ProjectileBase : Projectile
{
    private void FixedUpdate()
    {
        rigidBody.linearVelocity = transform.forward * speed;
    }
}
