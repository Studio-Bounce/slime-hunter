using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderSteeringBehaviour : SeekSteeringBehaviour
{
    public float wanderDistance = 2.0f;
    public float wanderRadius = 1.0f;
    public float wanderJitter = 20.0f;
    public RadialWanderBoundary wanderBounds;

    private Vector3 wanderTarget;

    private void Start()
    {
        float theta = (float)Random.value * Mathf.PI * 2;
        wanderTarget = new Vector3(wanderRadius * Mathf.Cos(theta),
                                   0.0f,
                                   wanderRadius * Mathf.Sin(theta));
    }

    public override Vector3 CalculateForce()
    {
        // Move the point within a small box
        float wanderJitterTimeSlice = wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitterTimeSlice,
                                                  0.0f,
                                                  Random.Range(-1.0f, 1.0f) * wanderJitterTimeSlice);
        
        // Normalize the point and put it back on the circle
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        // Move the point in local space out by the wander distance
        target = wanderTarget + new Vector3(0, 0, wanderDistance);

        // Move the target to world space in relationship to the agent
        target = steeringAgent.transform.rotation * target + steeringAgent.transform.position;

        // If out of bounds then wander back to the center
        if (wanderBounds != null && !wanderBounds.InBounds(transform.position))
        {
            target = wanderBounds.transform.position;
        }

        return base.CalculateSeekForce();
    }

    protected override void OnDrawGizmos()
    {

        Vector3 circle = transform.rotation * new Vector3(0, 0, wanderDistance) + transform.position;
        DebugExtension.DrawCircle(circle, Vector3.up, new Color(1, 0, 0, 0.8f), wanderRadius);
        Debug.DrawLine(transform.position, circle, new Color(1, 1, 0, 0.8f));
        Debug.DrawLine(transform.position, target, new Color(0, 0, 1, 0.8f));
    }
}
