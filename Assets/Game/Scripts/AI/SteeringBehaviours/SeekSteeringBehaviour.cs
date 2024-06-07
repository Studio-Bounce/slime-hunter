using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekSteeringBehaviour : SteeringBehaviourBase
{
    public bool performStuckDetection = false;

    protected Vector3 desiredVelocity;
    [SerializeField] float stopThreshold = 0.15f;

    private List<Vector3> positionCache = new List<Vector3>();
    [SerializeField] int cacheLength = 10;  // Store 10 previous positions
    private int currentCacheIdx = 0;

    private float prevCacheTime;
    [SerializeField] float cacheInterval = 0.25f;  // 250ms
    [SerializeField] float stuckThreshold = 0.01f;

    private void Start()
    {
        prevCacheTime = Time.time;
    }

    public override Vector3 CalculateForce()
    {
        // If agent walks over the target, stop it
        if (Vector3.Distance(transform.position, target) <= stopThreshold || IsStuck())
            steeringAgent.reachedGoal = true;

        if (performStuckDetection && Time.time > prevCacheTime + cacheInterval)
        {
            CachePosition();
            prevCacheTime = Time.time;
        }

        CheckMouseInput();
        return CalculateSeekForce();
    }

    protected Vector3 CalculateSeekForce()
    {
        desiredVelocity = (target - transform.position).normalized;
        desiredVelocity *= steeringAgent.maxSpeed;
        return (desiredVelocity - steeringAgent.velocity);
    }

    // ------------ Stuck Detection ------------
    // Sometimes the player can get stuck in loop of movement
    // When that happens, its best to stop the player

    private void CachePosition()
    {
        if (currentCacheIdx < cacheLength)
        {
            if (currentCacheIdx >= positionCache.Count)
                positionCache.Add(transform.position);
            else
                positionCache[currentCacheIdx] = transform.position;
        }
        currentCacheIdx = (currentCacheIdx + 1) % cacheLength;
    }

    private bool IsStuck()
    {
        // Don't perform stuck detection if we don't have enough position caches
        if (positionCache.Count == 0 || positionCache.Count < cacheLength)
            return false;

        // Check if the current transform is the same as the oldest position cached (next idx)
        int idx = (currentCacheIdx == positionCache.Count - 1) ? 0 : currentCacheIdx + 1;

        if ((transform.position - positionCache[idx]).sqrMagnitude < stuckThreshold)
        {
            Debug.Log("Agent is stuck! Current position = " + transform.position + " & Repeated position = " + positionCache[idx]);
            return true;
        }
        return false;
    }

    protected virtual void OnDrawGizmos()
    {
        if (steeringAgent != null)
        {
            DebugExtension.DebugArrow(transform.position, desiredVelocity, Color.red);
            DebugExtension.DebugArrow(transform.position, steeringAgent.velocity, Color.blue);
        }

        DebugExtension.DebugPoint(target, Color.magenta);

        // Cached positions
        for (int i = 0; i < positionCache.Count; i++)
        {
            DebugExtension.DebugPoint(positionCache[i], Color.red);
        }
    }
}