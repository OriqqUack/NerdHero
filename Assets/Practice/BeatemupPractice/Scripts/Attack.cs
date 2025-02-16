using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int attackCount = 1;
    
    private Entity _ownerEntity;
    private Effect _effect;
    private int _currentAttackCount = 0;

    private void OnEnable()
    {
        _currentAttackCount = 0;
    }

    public void Setup(Entity entity, Effect effect)
    {
        _ownerEntity = entity;
        _effect = effect;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (!entity || entity == _ownerEntity)
            return;
        if (_currentAttackCount >= attackCount)
            return;
        
        entity.TakeDamage(_ownerEntity, _effect, _ownerEntity.Stats.Damage.Value);
        _currentAttackCount++;
    }
}
