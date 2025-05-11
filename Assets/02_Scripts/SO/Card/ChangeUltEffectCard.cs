using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardUltEffectChange")]
public class ChangeUltEffectCard : CardBase
{
    [SerializeField] private Category category;

    private Effect _foundEffect;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);

        _foundEffect = _owner.SkillSystem.OwnSkills[1].ExtraAddEffects.Find(x => x.HasCategory(category));
    }

    public override void ApplyEffect()
    {
        _owner.SkillSystem.OwnSkills[1].ExtraAddEffects.Remove(_foundEffect);
        _owner.SkillSystem.OwnSkills[1].ExtraAddEffects.Add(_effect);
        SkillBar.Instance.slots[0].Skill = _owner.SkillSystem.OwnSkills[1];
    }
}
