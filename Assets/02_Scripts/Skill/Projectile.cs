using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject impactPrefab;
    [SerializeField] private List<AudioClip> impactSounds;
    [SerializeField] private AudioClip createSound;
    [SerializeField] private bool canPenetrate;
    
    [Space(10)][Header("Shadow Settings")]
    [SerializeField] private GameObject shadow;
    [SerializeField] private bool lockYAxis = false;
    [SerializeField] private float yOffset;

    protected Entity owner;
    protected Rigidbody rigidBody;
    protected float speed;
    protected Skill skill;
    protected Vector3 direction;
    public float Speed => speed;
    public virtual void Setup(Entity owner, float speed, Vector3 direction, Skill skill)
    {
        this.owner = owner;
        this.speed = speed;
        this.direction = direction;
        this.skill = skill.Clone() as Skill;
        transform.right = direction.normalized;
        if(!shadow)
        {
            GameObject go = Resources.Load<GameObject>("Prefabs/Shadow");
            shadow = Managers.Resource.Instantiate(go, transform.position, Quaternion.identity);
        }
        shadow.transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        shadow.transform.rotation = Quaternion.Euler(90, 0, 0);
        
        if(createSound)
            Managers.SoundManager.Play(createSound);
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    protected virtual void FixedUpdate()
    {
        if (!lockYAxis) return;
        shadow.transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
    }

    private void OnDestroy()
    {
        Destroy(skill);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!skill) return;
        Entity entity = other.gameObject.GetComponent<Entity>();

        if (entity)
        {
            if (entity == owner) return;
            if (entity.ControlType == owner.ControlType) return;
            
            entity.SkillSystem.Apply(skill);
        }
        else
        {
            if (other.CompareTag("Ground")) Managers.Resource.Destroy(gameObject);
            else return;
        }
        
        if (impactPrefab)
        {
            var impact = Instantiate(impactPrefab);
            impact.transform.forward = -transform.forward;
            impact.transform.position = transform.position;
        }

        if (impactSounds.Count >= 1)
        {
            int value = Random.Range(0, impactSounds.Count);
            Managers.SoundManager.Play(impactSounds[value]);
        }
        
        if(!canPenetrate)
        {
            Managers.Resource.Destroy(shadow);
            Managers.Resource.Destroy(gameObject);
        }

    }
}
