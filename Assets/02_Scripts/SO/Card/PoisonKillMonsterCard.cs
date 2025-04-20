using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardPoisonKillMonster")]
public class PoisonKillMonsterCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = _owner.SkillSystem.OwnSkills[0].Effects.Find(x => x.CodeName.Contains("POISON"));
        _owner.onKill += KillMonster;
    }

    public override void ApplyEffect()
    {
        
    }

    private void KillMonster(Entity entity)
    {
        if (!_effect) return;

        if (entity.SkillSystem.Find("POISON").Count != 0)
        {
            foreach (var monster in WaveManager.Instance.ActiveEnemies)
            {
                monster.SkillSystem.Apply(_effect);
            }
        }
    }
}
