using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
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

    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null || !child.gameObject.activeInHierarchy)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
