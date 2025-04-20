using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardUseUlt")]
public class UseUltCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        _owner.SkillSystem.OwnSkills[1].onUsed += UsedSkill;
    }

    private void UsedSkill(Skill skill)
    {
        _owner.SkillSystem.Apply(_effect);
    }
}
