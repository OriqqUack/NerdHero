using Spine;
using Spine.Unity;
using UnityEngine;

[System.Serializable]
public class SyncPosition : CustomAction
{
    private Entity _entity;
    private Transform _targetTs;
    public override void Start(object data)
    {
        _entity = (data as Skill)?.Owner;
        _targetTs = _entity.GetTransformSocket("pelvis");
    }

    public override void Run(object data)
    {
    }

    public override void Release(object data)
    {
        _entity.transform.position = _targetTs.position;
    }
    

    public override object Clone()
    {
        throw new System.NotImplementedException();
    }
}
