using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private bool isBouncing;

    protected Entity owner;
    protected Rigidbody rigidBody;
    protected float speed;
    protected Skill skill;

    public virtual void Setup(Entity owner, float speed, Vector3 direction, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;
        transform.forward = direction;
        this.skill = skill.Clone() as Skill;
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        Destroy(skill);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!skill) return;

        if (!other.CompareTag("Ground"))
        {
            Entity entity = other.gameObject.GetComponent<Entity>();
            if (!entity) return;
            if (entity == owner) return;
            if (entity.ControlType == owner.ControlType) return;
            
            if (entity)
                entity.SkillSystem.Apply(skill);
        }

        if (impactPrefab)
        {
            var impact = Instantiate(impactPrefab);
            impact.transform.forward = -transform.forward;
            impact.transform.position = transform.position;
        }

        if(!isBouncing)
            Destroy(gameObject);
    }
}
