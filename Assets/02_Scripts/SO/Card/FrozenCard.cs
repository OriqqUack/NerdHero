using UnityEngine;
[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardFrozen")]
public class FrozenCard : CardBase
{
    [SerializeField] private Effect secondEffect;
    private Effect _secondEffectClone;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
        _secondEffectClone = secondEffect.Clone() as Effect;
        _secondEffectClone.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        var list = _owner.SkillSystem.Find("ICE");
        foreach (var effect in list)
        {
            _owner.SkillSystem.RemoveEffect(effect);
        }
        
        _owner.SkillSystem.Apply(_effect);
        _owner.SkillSystem.Apply(_secondEffectClone);
    }
}
