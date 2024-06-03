using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use for spawning a boundary for wandering slimes 
public class BoundarySpawner : MonoBehaviour
{
    [SerializeField] float distance = 5.0f;

    [Header("Collision properties")]
    [SerializeField] LayerMask includeLayers;
    [SerializeField] LayerMask excludeLayers;

    void Start()
    {
        Vector3 forwardVec = transform.forward;
        Vector3 backwardVec = new(-forwardVec.x, forwardVec.y, -forwardVec.z);
        Vector3 rotated1 = new(-forwardVec.z, forwardVec.y, forwardVec.x);
        Vector3 rotated2 = new(forwardVec.z, forwardVec.y, -forwardVec.x);

        Vector3[] wallPositions = { forwardVec, backwardVec, rotated1, rotated2 };
        Vector3[] wallRotations = { new(90, 0, 0), new(90, 0, 0), new(0, 0, 90), new(0, 0, 90) };

        // Spawn the walls
        for (int i = 0; i < wallPositions.Length; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);
            wall.transform.SetPositionAndRotation(transform.position + distance * wallPositions[i],
                                                  Quaternion.Euler(wallRotations[i]));

            // In unity, plane length by default is 10x10
            float scale = 2 * distance / 10f;
            if (wallRotations[i].x > 0)
            {
                wall.transform.localScale = new(scale, 1, 1);
            }
            else
            {
                wall.transform.localScale = new(1, 1, scale);
            }

            // Make it child
            wall.transform.parent = transform;

            MeshCollider meshCollider = wall.GetComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.includeLayers = includeLayers;
            meshCollider.excludeLayers = excludeLayers;

            // Remove mesh renderer
            MeshRenderer meshRenderer = wall.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Destroy(meshRenderer);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 forwardVec = transform.forward;
        Vector3 backwardVec = new(-forwardVec.x, forwardVec.y, -forwardVec.z);
        Vector3 rotated1 = new(-forwardVec.z, forwardVec.y, forwardVec.x);
        Vector3 rotated2 = new(forwardVec.z, forwardVec.y, -forwardVec.x);

        DebugExtension.DebugArrow(transform.position, forwardVec * distance);
        DebugExtension.DebugArrow(transform.position, backwardVec * distance);
        DebugExtension.DebugArrow(transform.position, rotated1 * distance);
        DebugExtension.DebugArrow(transform.position, rotated2 * distance);
    }
}
