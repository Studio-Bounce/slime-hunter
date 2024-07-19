using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxWanderBoundary : WanderBoundary
{
    public Vector2 innerSize;
    public float outerSizeOffset;
    public Color color = new Color(1, 1, 1, 0.8f);

    public override bool InBounds(Vector3 pos)
    {
        Vector3 localPos = transform.InverseTransformPoint(pos);
        Vector2 halfSize = innerSize * 0.5f;
        return Mathf.Abs(localPos.x) < halfSize.x && Mathf.Abs(localPos.z) < halfSize.y;
    }

    public override bool InOuterBounds(Vector3 pos)
    {
        Vector3 localPos = transform.InverseTransformPoint(pos);
        Vector2 halfSize = (innerSize + new Vector2(outerSizeOffset, outerSizeOffset)) * 0.5f;
        return Mathf.Abs(localPos.x) < halfSize.x && Mathf.Abs(localPos.z) < halfSize.y;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = color;
        Vector3 center = transform.position;
        Vector3 halfSize = new Vector3(innerSize.x / 2, 0, innerSize.y / 2);

        // Transform corners
        Vector3[] corners = new Vector3[5];
        corners[0] = transform.TransformPoint(new Vector3(-halfSize.x, 0, -halfSize.z)); // Bottom left
        corners[1] = transform.TransformPoint(new Vector3(-halfSize.x, 0, halfSize.z));  // Top left
        corners[2] = transform.TransformPoint(new Vector3(halfSize.x, 0, halfSize.z));   // Top right
        corners[3] = transform.TransformPoint(new Vector3(halfSize.x, 0, -halfSize.z));  // Bottom right
        corners[4] = corners[0]; // Close the loop

        Handles.DrawAAPolyLine(5, corners);

        halfSize = new Vector3((innerSize.x + outerSizeOffset) / 2, 0, (innerSize.y + outerSizeOffset) / 2);
        corners[0] = transform.TransformPoint(new Vector3(-halfSize.x, 0, -halfSize.z)); // Bottom left
        corners[1] = transform.TransformPoint(new Vector3(-halfSize.x, 0, halfSize.z));  // Top left
        corners[2] = transform.TransformPoint(new Vector3(halfSize.x, 0, halfSize.z));   // Top right
        corners[3] = transform.TransformPoint(new Vector3(halfSize.x, 0, -halfSize.z));  // Bottom right
        corners[4] = corners[0]; // Close the loop

        Handles.DrawAAPolyLine(10, corners);
    }
#endif
}
