using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/Kill Monster")]
public class KillMonsterCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
        _owner.onKill += KillMonster;
    }

    public override void ApplyEffect()
    {
        _owner.SkillSystem.Apply(_effect);
    }

    private void KillMonster(Entity entity)
    {
        ApplyEffect();
    }
}
