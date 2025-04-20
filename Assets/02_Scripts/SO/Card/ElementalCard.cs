using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardElemental")]
public class ElementalCard : CardBase
{
    [SerializeField] private string elementalName;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        var list = _owner.SkillSystem.Find(elementalName);
        foreach (var effect in list)
        {
            _owner.SkillSystem.RemoveEffect(effect);
        }
        
        _owner.SkillSystem.Apply(_effect);
    }
}
