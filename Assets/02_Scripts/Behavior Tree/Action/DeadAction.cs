using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;


public class DeadAction : EnemyAction
{
    public override void OnStart()
    {
        entityMovement.Stop();
        GameObject.Destroy(gameObject, 3.0f);
        gameObject.GetComponent<BehaviorTree>().enabled = false;
    }
}


