using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardRevive")]
public class ReviveCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        _owner.onDead += Revive;
    }

    private void Revive(Entity entity)
    {
        _owner.GetComponent<PlayerMovement>().ReviveEvent();
    }
}
