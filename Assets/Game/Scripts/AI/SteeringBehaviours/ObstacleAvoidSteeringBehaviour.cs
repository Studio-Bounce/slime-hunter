using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidSteeringBehaviour : SteeringBehaviourBase
{
    [System.Serializable]

    public class Feeler
    {
        public float distance;
        public Vector3 offset;
    };

    public List<Feeler> feelers = new List<Feeler>();
    public LayerMask layerMask;

    public float avoidWeight = 1.0f;

    public override Vector3 CalculateForce()
    {
        RaycastHit hit;
        Ray ray;

        foreach (Feeler feeler in feelers)
        {
            Vector3 feelerPosition = transform.rotation * feeler.offset + transform.position;
            ray = new Ray(feelerPosition, transform.forward);

            if (Physics.Raycast(ray, out hit, feeler.distance, layerMask.value))
            {
                Vector3 forceDirection = Vector3.Project(hit.point - transform.position, transform.forward);
                float multiplier = 1.0f + ((feeler.distance - forceDirection.magnitude) / feeler.distance);

                Vector3 forcePosition = forceDirection + transform.position;
                forceDirection = (forcePosition - hit.point).normalized * multiplier * (1.0f / avoidWeight);
                return forceDirection;
            }
        }

        return Vector3.zero;
    }

    protected virtual void OnDrawGizmos()
    {
        foreach (Feeler feeler in feelers)
        {
            Vector3 feelerPosition = transform.localRotation * feeler.offset + transform.position;
            Debug.DrawLine(feelerPosition, transform.forward * feeler.distance + feelerPosition, Color.blue);
        }
    }
}
