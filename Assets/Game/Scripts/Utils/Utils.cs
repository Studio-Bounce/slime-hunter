using Ink.Runtime;
using System;
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

    public static Vector2 PlayerToScreenPoint()
    {
        return CameraManager.ActiveCamera.WorldToScreenPoint(GameManager.Instance.PlayerRef.transform.position);
    }

    public static bool IsWorldPositionOffScreen(Vector3 worldPosition, out Vector3 screenPosition)
    {
        screenPosition = CameraManager.ActiveCamera.WorldToScreenPoint(worldPosition);
        return IsScreenPositionOffScreen(screenPosition);
    }

    public static bool IsScreenPositionOffScreen(Vector3 screenPosition)
    {
        return (screenPosition.x < 0 || screenPosition.x > Screen.width ||
                screenPosition.y < 0 || screenPosition.y > Screen.height);
    }

    public static string ToCapitalizedString(string text)
    {
        text.Trim();
        if (text.Length > 1)
        {
            return char.ToUpper(text[0]) + text.Substring(1).ToLower();
        }
        else if (text.Length == 1)
        {
            return text.ToUpper();
        }
        else
        {
            return text;
        }
    }
}
