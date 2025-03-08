using UnityEngine;

public class ProjectileParabolic : Projectile
{
    [SerializeField] private float angle;
    public override void Setup(Entity owner, float speed, Vector3 direction, Skill skill)
    {
        base.Setup(owner, speed, direction, skill);
        FireProjectile(direction);
    }

    private void FireProjectile(Vector3 direction)
    {
        Vector3 rotatedDirection = Quaternion.AngleAxis(angle, Vector3.Cross(direction, Vector3.up)) * direction;
        Vector3 forceDirection = rotatedDirection.normalized * speed;
        rigidBody.AddForce(forceDirection, ForceMode.Impulse);
    }
}