using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;
[System.Serializable]
public class DestroySelfAction : CustomAction
{
    public override void Release(object data)
    {
        Skill skill = (Skill)data;
        if (!skill) return;
        WaveManager.Instance.RemoveEnemy(skill.Owner);
        GameObject.Destroy(skill.Owner.transform.parent.gameObject);
    }

    public override object Clone() => new DestroySelfAction();
}
