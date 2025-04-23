using System;
using Unity.VisualScripting;
using UnityEngine;

public enum AttributeType
{
    BaseAttack,
    Ult,
    Survive,
    Element,
    Orb,
    None
}

[CreateAssetMenu(fileName = "Card_", menuName = "Card/CardBase")]
public abstract class CardBase : ScriptableObject, ICloneable
{
    public AttributeType attributeType;
    [SerializeField] protected Effect effect;
    protected Effect _effect;
    
    [SerializeReference, SubclassSelector]
    protected CardCondition[] useConditions;

    protected Entity _owner;

    public Effect Effect => _effect;
    public Effect EffectSO => effect;
    
    public abstract void Setup(Entity entity);

    public abstract void ApplyEffect();

    public virtual object Clone()
    {
        var clone = Instantiate(this);
        
        _owner = WaveManager.Instance.PlayerEntity;
        if (_owner != null)
            clone.Setup(_owner);

        return clone;
    }
}
