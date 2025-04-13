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
    private int spawnCount;
    [SerializeField] 
    private float intervalTime;
    [SerializeField]
    private float speed;
    
    public override void Apply(Skill skill)
    {
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        skill.Owner.StartCoroutine(SpawnWithInterval(socket, skill));
    }
    
    private IEnumerator SpawnWithInterval(Transform socket, Skill skill)
    {
        for(int i = 0 ; i < spawnCount ; i++)
        {
            var projectile = GameObject.Instantiate(projectilePrefab);
            projectile.transform.position = socket.position;
            projectile.GetComponent<Projectile>().Setup(skill.Owner, speed, socket.forward, skill);

            yield return new WaitForSeconds(intervalTime);
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