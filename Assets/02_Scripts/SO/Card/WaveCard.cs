using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardWave")]
public class WaveCard : CardBase
{
    public override void Setup(Entity entity)
    {
        _owner = entity;
        _effect = effect.Clone() as Effect;
        _effect.Setup(_owner.gameObject, _owner, 1);
    }

    public override void ApplyEffect()
    {
        WaveManager.Instance.OnWaveChange += ChangeWave;
        ChangeWave(0);
    }

    private void ChangeWave(int index)
    {
        _owner.SkillSystem.Apply(_effect);
    }
}
