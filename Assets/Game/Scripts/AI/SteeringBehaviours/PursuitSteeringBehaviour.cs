using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitSteeringBehaviour : ArriveSteeringBehaviour
{
    public SteeringAgent pursuitObject;
    public Vector3 offset;

    public override Vector3 CalculateForce()
    {
        if (pursuitObject != null)
        {
            Vector3 worldSpaceOffset = pursuitObject.transform.rotation * offset + pursuitObject.transform.position;
            Vector3 offsetPosition = worldSpaceOffset - steeringAgent.transform.position;
            float lookAheadTime = offsetPosition.magnitude / (steeringAgent.maxSpeed + pursuitObject.velocity.magnitude);

            target = worldSpaceOffset + pursuitObject.velocity * lookAheadTime;
            return base.CalculateArriveForce();
        }

        return Vector3.zero;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        DebugExtension.DebugCircle(target, Vector3.up, Color.red, slowDownDistance);
    }
}
