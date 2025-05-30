using UnityEngine;

[System.Serializable]
public class SpawnColliderSkillObject : SkillAction
{
    [SerializeField]
    private GameObject skillPrefab;
    [SerializeField]
    private string spawnPointSocketName;

    [Header("Data")]
    [SerializeField]
    private float duration;
    [SerializeField]
    private float tickInterval;
    [SerializeField]
    private Vector3 objectScale = Vector3.one;
    
    public override void Apply(Skill skill)
    {
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        var skillObject = GameObject.Instantiate(skillPrefab);
        skillObject.transform.position = socket.position;
        skillObject.GetComponent<SkillColliderObject>().Setup(skill.Owner, skill, socket, duration,tickInterval, objectScale);
    }

    public override object Clone()
    {
        return new SpawnColliderSkillObject()
        {
            skillPrefab = skillPrefab,
            spawnPointSocketName = spawnPointSocketName,
            duration = duration,
            objectScale = objectScale
        };
    }
}
