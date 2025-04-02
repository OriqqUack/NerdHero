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
    }

    public override void Run(object data)
    {
        _entity.Animator?.PlayOneShot(AnimationName, Index);
    }

    public override object Clone()
    {
        return new PlayAnimationAction()
        {
            AnimationName = AnimationName,
        };
    }
}
