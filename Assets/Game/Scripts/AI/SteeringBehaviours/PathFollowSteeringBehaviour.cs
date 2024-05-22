using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFollowSteeringBehaviour : ArriveSteeringBehaviour
{
    public float waypointDistance = 0.5f;
    public int currentWaypointIndex = 0;
    public bool useNavmesh = true;

    [SerializeField] List<Transform> targets = new List<Transform>();
    public int targetIdx = 0;

    private NavMeshPath path;

    private void Start()
    {
        if (useNavmesh)
            path = new NavMeshPath();

        if (targets.Count > 0)
            target = targets[targetIdx].transform.position;
        else
            target = transform.position;  // Don't move
        GenerateNewPath();
    }

    private void Update()
    {
        if (steeringAgent.reachedGoal)
        {
            targetIdx = (targetIdx + 1) % targets.Count;
            target = targets[targetIdx].transform.position;
            GenerateNewPath();
        }
    }

    public override Vector3 CalculateForce()
    {
        if (path != null && useNavmesh && currentWaypointIndex != path.corners.Length && (target - transform.position).magnitude < waypointDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex < path.corners.Length)
            {
                target = path.corners[currentWaypointIndex];
            }
        }

        return CalculateArriveForce();
    }

    public void GenerateNewPath()
    {
        if (!useNavmesh)
            return;

        currentWaypointIndex = 0;
        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        if (path.corners.Length > 0)
        {
            target = path.corners[0];
        }
        else
        {
            target = transform.position;
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        DebugExtension.DrawCircle(target, Color.magenta, waypointDistance);
        if (path != null)
        {
            for (int i = 1; i < path.corners.Length; i++)
            {
                Debug.DrawLine(path.corners[i - 1], path.corners[i], Color.black);
            }
        }
    }
}
