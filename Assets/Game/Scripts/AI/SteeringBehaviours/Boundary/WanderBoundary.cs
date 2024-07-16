using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WanderBoundary : MonoBehaviour
{
    public bool lookForChildrenOnAwake = true;

    private void Awake()
    {
        if (lookForChildrenOnAwake) ApplyToChildren();
    }

    private void ApplyToChildren()
    {
        // Get all components of type WanderSteeringBehavior in the children of the current GameObject
        WanderSteeringBehaviour[] wanderComponents = GetComponentsInChildren<WanderSteeringBehaviour>();

        // Iterate through each component found and add reference to wanderBounds
        foreach (WanderSteeringBehaviour wanderComponent in wanderComponents)
        {
            wanderComponent.wanderBounds = this;
        }
    }

    public Vector3 Center { get { return transform.position; } }
    public abstract bool InBounds(Vector3 pos);
    public abstract bool InOuterBounds(Vector3 pos);
}
