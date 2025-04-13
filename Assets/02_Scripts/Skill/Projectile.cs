using System;
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
    
    [Space(10)][Header("Shadow Settings")]
    [SerializeField] private GameObject shadow;
    [SerializeField] private bool lockXAxis = false;
    [SerializeField] private float yOffset;

    protected Entity owner;
    protected Rigidbody rigidBody;
    protected float speed;
    protected Skill skill;
    protected Vector3 direction;
    public virtual void Setup(Entity owner, float speed, Vector3 direction, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;
        this.direction = direction;
        this.skill = skill.Clone() as Skill;
        transform.right = direction.normalized;
        shadow.transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        shadow.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        shadow.transform.position = new Vector3(transform.position.x, yOffset , transform.position.z);
    }

    protected virtual void FixedUpdate()
    {
        if (lockXAxis) return;
        shadow.transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
    }

    private void OnDestroy()
    {
        Destroy(skill);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!skill) return;

        if (impactPrefab)
        {
            var impact = Instantiate(impactPrefab);
            impact.transform.forward = -transform.forward;
            impact.transform.position = transform.position;
        }
        
        if (other.CompareTag("Ground")) Managers.Resource.Destroy(gameObject);
        
        Entity entity = other.gameObject.GetComponent<Entity>();
        if (!entity) return;
        if (entity == owner) return;
        if (entity.ControlType == owner.ControlType) return;
        
        if (entity)
            entity.SkillSystem.Apply(skill);

        if(!isBouncing)
            Managers.Resource.Destroy(gameObject);
    }
}
