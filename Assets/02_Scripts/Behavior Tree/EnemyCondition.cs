using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EnemyCondition : Conditional
{
    protected Entity entity;
    protected EntityMovement entityMovement;
    
    public override void OnAwake()
    {
        entity = GetComponent<Entity>();
        entityMovement = GetComponent<Movement>() as EntityMovement;
    }
}
