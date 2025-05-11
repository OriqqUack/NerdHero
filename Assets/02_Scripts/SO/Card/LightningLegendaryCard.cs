using UnityEngine;
[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardElementalLightningLegendary")]
public class LightningLegendaryCard : CardBase
{
    [SerializeField] private string elementalName;
    public override void Setup(Entity entity)
    {
        _owner = entity;
        
        var list = _owner.SkillSystem.Find(elementalName);
        bool isFind = list.Count > 0;
        
        _effect = effect.Clone() as Effect;
        int index = 1;
        if (isFind)
        {
            if (list.Count == 1)
            {
                index = list[0].CodeName.Contains("COMMON") ? 2 : 3;
            }
            else
            {
                index = 4;
            }
        }
        
        _effect.Setup(_owner.gameObject, _owner, index);
    }

    public override void ApplyEffect()
    {
        var list = _owner.SkillSystem.Find(elementalName);
        foreach (var effect in list)
        {
            _owner.SkillSystem.RemoveEffect(effect);
        }
        
        _owner.SkillSystem.OwnSkills[0].ExtraAddEffects.Add(_effect);
    }
}
