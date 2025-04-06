using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProjectilesAction : SkillAction
{
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private string spawnPointSocketName;
    [SerializeField] 
    private int intervalTime;
    [SerializeField]
    private float speed;
    
    public override void Apply(Skill skill)
    {
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        foreach (Transform spawnPoint in socket)
        {
            var projectile = GameObject.Instantiate(projectilePrefab);
            projectile.transform.position = spawnPoint.position;
            projectile.GetComponent<Projectile>().Setup(skill.Owner, speed, spawnPoint.forward, skill);
        }
    }

    public override object Clone()
    {
        return new SpawnProjectilesAction()
        {
            projectilePrefab = projectilePrefab,
            spawnPointSocketName = spawnPointSocketName,
            speed = speed
        };
    }
}