using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardEffectAdd")]
public class UltEffectAddCard : CardBase
{
    [SerializeField] private int index;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        _owner.SkillSystem.OwnSkills[index].AddEffect(_effect);
    }
}

