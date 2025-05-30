using System;
using UnityEngine;

public class Tackle : MonoBehaviour
{
    private Entity _entity;

    private void Awake()
    {
        _entity = GetComponent<Entity>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Entity entity = other.gameObject.GetComponent<Entity>();
            float damage = _entity.Stats.GetStat("DAMAGE").Value;
            entity.TakeDamage(_entity, true, damage);
        }
    }
}
