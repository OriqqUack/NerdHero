using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyCondition : Conditional
{
    protected Entity entity;
    protected EntityMovement entityMovement;
    protected Transform playerTransform;
    
    public override void OnAwake()
    {
        entity = GetComponent<Entity>();
        entityMovement = GetComponent<Movement>() as EntityMovement;
        playerTransform = WaveManager.Instance.PlayerTransform;
    }
}
