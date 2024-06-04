using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArriveSteeringBehaviour : SeekSteeringBehaviour
{
    public float slowDownDistance = 2.0f;
    public float stoppingDistance = 0.1f;

    public override Vector3 CalculateForce()
    {
        CheckMouseInput();
        return CalculateArriveForce();
    }

    protected Vector3 CalculateArriveForce()
    {
        Vector3 toTarget = target - steeringAgent.transform.position;
        float distance = toTarget.magnitude;

        steeringAgent.reachedGoal = false;
        if (distance > slowDownDistance)
        {
            return base.CalculateSeekForce();
        }
        else if (distance <= slowDownDistance && distance > stoppingDistance)
        {
            toTarget.Normalize();

            float speed = steeringAgent.maxSpeed * (distance / slowDownDistance);
            speed = (speed < steeringAgent.maxSpeed ? speed : steeringAgent.maxSpeed);
            desiredVelocity = (speed / distance) * toTarget;
            return desiredVelocity - steeringAgent.velocity;
        }

        steeringAgent.reachedGoal = true;
        return Vector3.zero;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        DebugExtension.DebugCircle(target, Vector3.up, Color.red, slowDownDistance);
    }
}
