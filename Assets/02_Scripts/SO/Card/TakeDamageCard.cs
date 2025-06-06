using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/TakeDamageCard")]
public class TakeDamageCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        _owner.onTakeDamage += (entity, instigator, causer, damage) => _owner.SkillSystem.Apply(_effect);
    }
    
    
}
