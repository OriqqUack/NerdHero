using UnityEngine;

public class OrbGroup
{
    public GameObject orbObjectPrefab;
    public int count;
}

[System.Serializable]
public class SpawnOrbAction : EffectAction
{
    [SerializeField]
    private GameObject orbObjectPrefab;
    [SerializeField]
    private int applyCount;

    public override bool Apply(Effect effect, Entity user, Entity target, int level, int stack, float scale)
    {
        OrbGroup group = new OrbGroup();
        group.orbObjectPrefab = orbObjectPrefab;
        group.count = applyCount;
        OrbSpawner.Instance.AddOrb(group, user);
        
        return true;
    }

    public override object Clone()
    {
        return new SpawnOrbAction()
        {
            applyCount = applyCount,
            orbObjectPrefab = orbObjectPrefab
        };
    }
}
