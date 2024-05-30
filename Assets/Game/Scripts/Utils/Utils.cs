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
}
