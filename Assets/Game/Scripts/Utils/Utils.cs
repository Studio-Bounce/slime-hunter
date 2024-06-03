using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 DirectionToCameraForward(Vector3 targetPosition, Vector2 targetDirection)
    {
        // Get forward based on camera
        Vector3 cameraToPlayer = (targetPosition - Camera.main.transform.position);
        Vector2 forwardDirection = new Vector2(cameraToPlayer.x, cameraToPlayer.z);
        forwardDirection.Normalize();
        Vector2 rightDirection = new Vector2(forwardDirection.y, -forwardDirection.x);

        // Calculate movement direction based on forward
        Vector2 direction2D = (forwardDirection * targetDirection.y + rightDirection * targetDirection.x).normalized;
        return new Vector3(direction2D.x, 0, direction2D.y);
    }

    public static bool CheckTimer(ref float _timer, float value)
    {
        if (_timer > value)
        {
            _timer = 0;
            return true;
        }
        return false;
    }

    public static float UnclampedLerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }
    public static Vector3 UnclampedLerp(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(
            UnclampedLerp(a.x, b.x, t),
            UnclampedLerp(a.y, b.y, t),
            UnclampedLerp(a.z, b.z, t)
        );
    }
}
