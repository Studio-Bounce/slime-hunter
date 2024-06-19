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

    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    // Credits to Freya Holmer
    // Returns a fraction t based off a value between a start and end
    public static float InvLerp(float a, float b, float v)
    {
        return (v - a) * (b - a);
    }
    public static float InvLerp(Vector3 a, Vector3 b, Vector3 v)
    {
        Vector3 range = b - a;
        Vector3 invLerp = v - a;
        float t = Vector3.Dot(invLerp, range) / Vector3.Dot(range, range);
        return t;
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

    public static bool IsVector3OffScreen(Vector3 pos)
    {
        return (pos.x < 0 || pos.x > Screen.width || pos.y < 0 || pos.y > Screen.height);
    }
}
