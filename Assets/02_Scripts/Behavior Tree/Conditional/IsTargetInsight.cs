using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class IsTargetInsight : EnemyCondition
{
    [UnityEngine.Tooltip("The maximum angle (in degrees) that the target can be within to be considered in sight")]
    public float fieldOfViewAngle = 45f;
    [UnityEngine.Tooltip("The maximum distance within which the target can be seen")]
    public float viewDistance = 10f;
    
    public override TaskStatus OnUpdate()
    {
        if (playerTransform != null && IsTargetInSight())
        {
            entity.Target = playerTransform.GetComponent<Entity>();
            entityMovement.IsFind = true;
            return TaskStatus.Success;
        }
        
        return TaskStatus.Failure;
    }

    /// <summary>
    /// Check if the target is within the agent's field of view and distance.
    /// </summary>
    private bool IsTargetInSight()
    {
        Vector3 directionToTarget = playerTransform.position - this.transform.position;

        // Check if the target is within the view distance
        if (directionToTarget.magnitude > viewDistance)
        {
            return false;
        }

        // Normalize the direction to target
        directionToTarget.Normalize();

        // Check if the target is within the field of view angle
        float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);
        if (angleToTarget < fieldOfViewAngle / 2f)
        {
            // Optional: Check for obstacles using raycast
            // if (!Physics.Linecast(agentTransform.position, target.Value.transform.position)) {
            //     return true;
            // }
            (entity.Movement as EntityMovement)?.SetTraceTarget(playerTransform);
            return true; // Target is within the field of view
        }

        return false; // Target is outside the field of view
    }

    /*public override void OnDrawGizmos()
    {
        // Set Gizmos color
        Gizmos.color = Color.yellow;

        // Draw the view distance as a circle
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        // Calculate the forward direction
        Vector3 forward = transform.forward;

        // Calculate the left and right bounds of the field of view
        Quaternion leftRotation = Quaternion.Euler(0, -fieldOfViewAngle / 2f, 0);
        Quaternion rightRotation = Quaternion.Euler(0, fieldOfViewAngle / 2f, 0);

        Vector3 leftBoundary = leftRotation * forward * viewDistance;
        Vector3 rightBoundary = rightRotation * forward * viewDistance;

        // Draw the field of view lines
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);

        // Fill the field of view (optional, requires Unity EditorHandles for a filled arc)
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        for (float i = -fieldOfViewAngle / 2f; i < fieldOfViewAngle / 2f; i += 1f)
        {
            Vector3 start = Quaternion.Euler(0, i, 0) * forward * viewDistance;
            Vector3 end = Quaternion.Euler(0, i + 1f, 0) * forward * viewDistance;
            Gizmos.DrawLine(transform.position + start, transform.position + end);
        }
    }*/
}
