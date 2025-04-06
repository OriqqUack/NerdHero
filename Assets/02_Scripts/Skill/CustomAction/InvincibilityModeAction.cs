using UnityEngine;

[System.Serializable]
public class InvincibilityModeAction : CustomAction
{
    private Entity _entity;

    public override void Start(object data)
    {
        _entity = (data as Skill)?.Owner;
        _entity.CanTakeDamage = false;
        Debug.Log("HI");
    }

    public override object Clone()
    {
        return new InvincibilityModeAction()
        {
            
        };
    }
}
