using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WanderRadialBounds : MonoBehaviour
{
    public float radius;

    public bool InBounds(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < radius;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius, 4);
    }
#endif
}
