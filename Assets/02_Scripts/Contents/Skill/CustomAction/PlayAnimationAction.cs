using Spine;
using Spine.Unity;
using UnityEngine;

[System.Serializable]
public class PlayAnimationAction : CustomAction
{
    public string AnimationName;
    public int Index;
    
    private Entity _entity;
    public override void Start(object data)
    {
        _entity = (data as Skill)?.Owner;
        _entity.Animator?.PlayOneShot(AnimationName, Index, 1f);
    }

    public override object Clone()
    {
        return new PlayAnimationAction()
        {
            AnimationName = AnimationName,
        };
    }
}
