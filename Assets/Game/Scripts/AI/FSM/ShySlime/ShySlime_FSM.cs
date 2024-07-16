using System.Collections;
using System.Collections.Generic;
using UnityEngine;

{
    [Header("ShySlime Properties")]
    public readonly int FleePlayerStateName = Animator.StringToHash("FleePlayer");
    public FleeSteeringBehaviour fleeSteeringBehaviour;
    [Tooltip("Slime flees if player gets in proximity")]
    public float playerProximityDistance = 5.0f;
    public float fleeSpeed = 5.0f;

    protected override void OnDrawGizmos()
    {
        DebugExtension.DrawCircle(transform.position, new Color(0, 1, 1, 0.9f), playerProximityDistance);
    }
}
