using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Items")]
    public List<ItemSO> items;
    public GameObject droppedItemPrefab;

    [Header("Spawning")]
    public float launchAngle;
    public float angleRange;

    public void SpawnItems()
    {
        foreach (var item in items)
        {
            GameObject go = Instantiate(droppedItemPrefab, gameObject.scene) as GameObject;
            DroppedItem droppedItem = go.GetComponent<DroppedItem>();
            droppedItem.transform.position = transform.position;
            droppedItem.launchDirection = transform.forward;
            droppedItem.launchAngle = launchAngle;
            droppedItem.angleRange = angleRange;
            droppedItem.ItemRef = item;
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Define the direction vector (initially pointing in the Z direction)
        Vector3 direction = transform.forward;

        // Rotate the direction vector around the Y-axis by the specified angle
        Quaternion rotation = Quaternion.Euler(0, launchAngle, 0);
        Vector3 rotatedDirection = rotation * direction;

        // Define the start and end points of the arrow
        Vector3 startPoint = transform.position;
        Vector3 endPoint = startPoint + rotatedDirection;

        // Draw the main line of the arrow
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPoint, endPoint);

        // Draw the arrowhead
        float arrowHeadLength = 0.2f;
        float arrowHeadAngle = 20f;

        Vector3 right = Quaternion.LookRotation(rotatedDirection) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(rotatedDirection) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Gizmos.DrawLine(endPoint, endPoint + right * arrowHeadLength);
        Gizmos.DrawLine(endPoint, endPoint + left * arrowHeadLength);
    }
#endif
}
