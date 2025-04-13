using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnProjectilesBySocketsAction : SkillAction
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
        skill.Owner.StartCoroutine(SpawnWithInterval(socket, skill));
    }
    
    private IEnumerator SpawnWithInterval(Transform sockets, Skill skill)
    {
        foreach (Transform spawnPoint in sockets)
        {
            var projectile = GameObject.Instantiate(projectilePrefab);
            projectile.transform.position = spawnPoint.position;
            projectile.GetComponent<Projectile>().Setup(skill.Owner, speed, spawnPoint.forward, skill);

            yield return new WaitForSeconds(intervalTime);
        }
    }

    public override object Clone()
    {
        return new SpawnProjectilesBySocketsAction()
        {
            projectilePrefab = projectilePrefab,
            spawnPointSocketName = spawnPointSocketName,
            speed = speed
        };
    }
}