using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeSteeringBehaviour : SteeringBehaviourBase
{
    public Transform enemyTarget;
    [Tooltip("Max distance to flee. Not considered if 0")]
    public float fleeDistance = 5.0f;
    Vector3 desiredVelocity;
    private bool showGizmoArrows = true;

    public override Vector3 CalculateForce()
    {
        if (enemyTarget != null)
        {
            target = enemyTarget.position;
        }
        else
        {
            CheckMouseInput();
        }

        float distance = (transform.position - target).magnitude;
        if (fleeDistance != 0 && distance > fleeDistance)
        {
            showGizmoArrows = false;
            return Vector3.zero;
        }
        showGizmoArrows = true;

        desiredVelocity = (transform.position - target).normalized;
        desiredVelocity *= steeringAgent.maxSpeed;
        return (desiredVelocity - steeringAgent.velocity);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmoArrows)
            return;

        if (steeringAgent != null)
        {
            DebugExtension.DebugArrow(transform.position, desiredVelocity, Color.red);
            DebugExtension.DebugArrow(transform.position, steeringAgent.velocity, Color.blue);
        }

        DebugExtension.DebugPoint(target, Color.magenta);
    }
}
