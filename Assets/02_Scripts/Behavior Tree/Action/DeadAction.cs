using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;


public class DeadAction : EnemyAction
{
    [SerializeField] private GameObject dropItem;
    public override void OnStart()
    {
        entityMovement.Stop();
        UnityEngine.GameObject.Instantiate(dropItem, transform.position, Quaternion.identity);
        GameObject.Destroy(gameObject, 3.0f);
        gameObject.GetComponent<BehaviorTree>().enabled = false;
    }
}


