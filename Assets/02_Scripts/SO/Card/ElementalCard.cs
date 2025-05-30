using UnityEngine;

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardElemental")]
public class ElementalCard : CardBase
{
    [SerializeField] private string elementalName;
    
    public override void Setup(Entity entity)
    {
        _owner = entity;
        
        var list = _owner.SkillSystem.Find(elementalName);
        bool isFind = list.Count > 0;
        
        _effect = effect.Clone() as Effect;
        if(isFind)
            _effect.Setup(_owner.gameObject, _owner, 2);
        else
            _effect.Setup(_owner.gameObject, _owner, 1);
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
