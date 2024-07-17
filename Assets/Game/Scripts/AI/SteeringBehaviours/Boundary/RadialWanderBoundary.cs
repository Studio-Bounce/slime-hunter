using UnityEngine;
using UnityEditor;

public class RadialWanderBoundary : WanderBoundary
{
    public float innerRadius;
    public float outerRadiusOffset;
    public Color color = new Color(1, 1, 1, 0.8f);

    public override bool InBounds(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < innerRadius;
    }

    public override bool InOuterBounds(Vector3 pos)
    {
        return Vector3.Distance(transform.position, pos) < innerRadius+outerRadiusOffset;

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = color;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, innerRadius, 2);
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, innerRadius+outerRadiusOffset, 4);
    }
#endif
}
