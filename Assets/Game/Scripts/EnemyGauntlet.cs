using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class EnemyGauntlet : MonoBehaviour
{
    public Vector2 boundSize;
    private float height = 5f;
    public string collisionTag = "Player";
    public GameObject wallPrefab;
    public float wallWidth = 2f;
    public float spawnDelay = 0f;
    public bool active = true;
    
    BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = gameObject.AddComponent<BoxCollider>();
    }

    private void Start()
    {
        Debug.Assert(wallPrefab != null);

        boxCollider.isTrigger = true;
        boxCollider.center = new Vector3(boxCollider.center.x, height / 2, boxCollider.center.z);
        boxCollider.size = new Vector3(boundSize.x, height, boundSize.y);
    }

    private void SpawnWalls()
    {
        Vector2Int sideDirection = new Vector2Int(1, 0);
        Vector2 positionOffset = new Vector3(boundSize.x / 2, boundSize.y / 2);
        Vector2Int rotationOffset = new Vector2Int(0, 90);

        for (int i = 0; i < 4; i++)
        {
            Vector3 directionalOffset = new Vector3(positionOffset.x * sideDirection.y, 0, positionOffset.y * sideDirection.x);
            GameObject wall = Instantiate(wallPrefab, transform.position + directionalOffset, Quaternion.identity);

            wall.transform.localScale = new Vector3(sideDirection.x == 0 ? boundSize.y : boundSize.x , height, wallWidth);
            wall.transform.rotation = Quaternion.Euler(new Vector3(0, (rotationOffset*sideDirection).magnitude, 0));

            // Rotate 90 deg to next side
            sideDirection = new Vector2Int(-sideDirection.y, sideDirection.x);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
            Debug.Log("Trigger entered with object tagged as '" + collisionTag + "'");
            if (active)
            {
                Invoke("SpawnWalls", spawnDelay);
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (!active)
        {
            Handles.color = new Color(0, 0, 0, 1);
        }
        // Get the position of the object
        Vector3 position = transform.position;

        // Calculate half width and half length
        float halfWidth = boundSize.x / 2f;
        float halfLength = boundSize.y / 2f;

        // Calculate the four corners of the rect
        Vector3 topLeft = position + new Vector3(-halfWidth, 0, halfLength);
        Vector3 topRight = position + new Vector3(halfWidth, 0, halfLength);
        Vector3 bottomLeft = position + new Vector3(-halfWidth, 0, -halfLength);
        Vector3 bottomRight = position + new Vector3(halfWidth, 0, -halfLength);

        // Draw the square using Handles.DrawSolidRectangleWithOutline
        Handles.DrawSolidRectangleWithOutline(new Vector3[] { topLeft, topRight, bottomRight, bottomLeft }, new Color(1, 0, 0, 0.05f), Color.red);
    }
}
