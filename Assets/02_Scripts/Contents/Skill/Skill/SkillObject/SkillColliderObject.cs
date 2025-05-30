using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Collider))]
public class SkillColliderObject : MonoBehaviour
{
    [SerializeField] private GameObject impactPrefab;
    [HideInInspector] public Vector3 ObjectScale;
    [SerializeField] private VisualEffect skillVFX;

    private Entity _owner;
    private Skill _skill;
    private float _duration;
    private float _tickInterval;
    
    private HashSet<Entity> entitiesInZone = new HashSet<Entity>(); // 현재 영역 내의 엔터티들
    private Dictionary<Entity, Coroutine> activeCoroutines = new Dictionary<Entity, Coroutine>();
    private float _currentTime;
    public void Setup(Entity entity, Skill skill, Transform parent, float duration, float tickInterval, Vector3 objectScale)
    {
        _owner = entity;
        _skill = skill;
        transform.forward = parent.forward;
        _duration = duration;
        _tickInterval = tickInterval;
        ObjectScale = objectScale;
        transform.parent = parent;

        skillVFX.SetFloat("Duration", duration);
        
        foreach (var component in GetComponents<ISkillObjectComponent>())
            component.OnSetupSkillObject(this);
    }

    private void Update()
    {
        _currentTime += Time.deltaTime;
        
        if(_currentTime >= _duration)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity == _owner) return;
        if (entity != null && !entitiesInZone.Contains(entity))
        {
            entitiesInZone.Add(entity);
            Coroutine damageCoroutine = StartCoroutine(ApplyTickDamage(entity));
            activeCoroutines[entity] = damageCoroutine;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity != null && entitiesInZone.Contains(entity))
        {
            entitiesInZone.Remove(entity);

            // 해당 엔터티의 코루틴이 실행 중이면 정지
            if (activeCoroutines.ContainsKey(entity))
            {
                StopCoroutine(activeCoroutines[entity]);
                activeCoroutines.Remove(entity);
            }
        }
    }
    
    private IEnumerator ApplyTickDamage(Entity entity)
    {
        while (entitiesInZone.Contains(entity))
        {
            entity.SkillSystem.Apply(_skill);
            if (impactPrefab)
            {
                GameObject go = Instantiate(impactPrefab);
                go.transform.position = entity.transform.position;
            }
            yield return new WaitForSeconds(_tickInterval); // 일정 시간 대기 후 반복
        }
    }
}
