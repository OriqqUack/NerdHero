using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardFireLegendary")]
public class FireLegendaryCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        throw new System.NotImplementedException();
    }
}
