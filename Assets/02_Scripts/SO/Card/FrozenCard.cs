using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardFrozen")]
public class FrozenCard : CardBase
{
    [SerializeField] private Effect secondEffect;
    private Effect _secondEffectClone;
    
    private List<Effect> _foundEffects;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _secondEffectClone = secondEffect.Clone() as Effect;

        _foundEffects = _owner.SkillSystem.Find("ICE");
        bool isFind = _foundEffects.Count > 0;

        if (isFind)
        {
            _effect.Setup(_owner.gameObject, _owner, 2);
            _secondEffectClone.Setup(_owner.gameObject, _owner, 2);
        }
        else
        {
            _effect.Setup(_owner.gameObject, _owner, 1);
            _secondEffectClone.Setup(_owner.gameObject, _owner, 1);
        }
    }

    public override void ApplyEffect()
    {
        if (_foundEffects.Count > 0)
        {
            foreach (Effect effect in _foundEffects)
            {
                _owner.SkillSystem.RemoveEffect(effect);
            }
        }
        
        _owner.SkillSystem.OwnSkills[0].AddedEffects.Add(_effect);
        _owner.SkillSystem.OwnSkills[0].AddedEffects.Add(_secondEffectClone);
    }
}
