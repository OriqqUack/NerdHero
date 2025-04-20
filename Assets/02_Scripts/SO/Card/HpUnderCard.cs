using UnityEngine;
[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardHPUnder")]
public class HpUnderCard : CardBase
{
    [SerializeField] private float hpRate = 0.5f;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        _owner.onTakeDamage += TakeDamage;
    }

    private void TakeDamage(Entity entity, Entity instigator, object causer, float damage)
    {
        float hp = entity.Stats.HPStat.Value;
        float maxHp = entity.Stats.HPStat.MaxValue;
        if (hp <= (maxHp * hpRate))
        {
            _owner.SkillSystem.Apply(_effect);
        }
        else
        {
            if (_owner.SkillSystem.Find(_effect))
            {
                _owner.SkillSystem.RemoveEffect(_effect);
            }
        }        
    }
}
