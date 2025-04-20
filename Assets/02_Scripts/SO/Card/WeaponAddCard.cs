using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/WeaponAddCard")]
public class WeaponAddCard : CardBase
{
    private Skill skill;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
        skill = entity.SkillSystem.OwnSkills[0];
    }

    public override void ApplyEffect()
    {
        (skill.CurrentData.action as SpawnProjectilesAction)?.AddSpawnCount(1);
    }
}
