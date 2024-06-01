using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class EnemyGauntlet : MonoBehaviour
{
    [Header("Gauntlet Bounds")]
    public Vector2 boundSize;
    public float boundHeight = 3f;
    public string collisionTag = "Player";
    public GameObject wallPrefab;
    public float wallWidth = 2f;
    public float colliderOffset;
    public float boundSpawnDelay = 0f;
    public bool active = true;

    private BoxCollider _boxCollider;

    [Header("Gauntlet Enemy Waves")]
    public List<EnemyWave> enemyWaves = new List<EnemyWave>();

    private void Awake()
    {
        _boxCollider = gameObject.AddComponent<BoxCollider>();
    }

    private void Start()
    {
        Debug.Assert(wallPrefab != null, "Requires a prefab wall");

        _boxCollider.isTrigger = true;
        _boxCollider.center = new Vector3(_boxCollider.center.x, boundHeight / 4, _boxCollider.center.z);
        _boxCollider.size = new Vector3(boundSize.x - colliderOffset, boundHeight / 2, boundSize.y - colliderOffset);
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

            wall.transform.localScale = new Vector3(sideDirection.x == 0 ? boundSize.y : boundSize.x , boundHeight, wallWidth);
            wall.transform.rotation = Quaternion.Euler(new Vector3(0, (rotationOffset*sideDirection).magnitude, 0));

            // Rotate 90 deg to next side
            sideDirection = new Vector2Int(-sideDirection.y, sideDirection.x);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
            if (active)
            {
                Invoke("SpawnWalls", boundSpawnDelay);
                StartCoroutine(StartEnemySpawn());
            }
        }

    }

    IEnumerator StartEnemySpawn()
    {
        foreach (EnemyWave wave in enemyWaves)
        {
            for (int i = 0; i < wave.totalEnemies; i++)
            {
                EnemySpawnProperties enemy = wave.RollEnemy();

                switch (enemy.spawnLocation)
                {
                    case SpawnLocation.RANDOM_BORDER:
                        SpawnAtBorder(enemy);
                        break;
                    case SpawnLocation.RANDOM_INNER:
                        SpawnAtInner(enemy);
                        break;
                    default:
                        break;
                }

                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
        yield return null;
    }

    private void SpawnAtBorder(EnemySpawnProperties enemy)
    {
        // Select a random border location
        int border = Random.Range(0, 4); // 0 = top, 1 = right, 2 = bottom, 3 = left
        float x = 0f;
        float y = 0f;
        Vector2 innerBound = new Vector2(boundSize.x/2 - colliderOffset, boundSize.y/2 - colliderOffset);
        switch (border)
        {
            case 0: // Top border
                x = Random.Range(-innerBound.x, innerBound.x);
                y = innerBound.y;
                break;
            case 1: // Right border
                x = innerBound.x;
                y = Random.Range(-innerBound.y, innerBound.y);
                break;
            case 2: // Bottom border
                x = Random.Range(-innerBound.x, innerBound.x);
                y = -innerBound.y;
                break;
            case 3: // Left border
                x = -innerBound.x;
                y = Random.Range(-innerBound.y, innerBound.y);
                break;
        }
        Vector3 borderPosition = transform.position + new Vector3(x, 0, y);
        // Instantiate the object at the border position
        Instantiate(enemy.enemyPrefab, borderPosition, Quaternion.identity);
        //Instantiate(enemy.enemyPrefab);
    }

    private void SpawnAtInner(EnemySpawnProperties enemy)
    {
        Vector2 innerBound = new Vector2(boundSize.x/2 - colliderOffset, boundSize.y/2 - colliderOffset);
        // Generate a random position within the specified area
        float randomX = Random.Range(-innerBound.x, innerBound.x);
        float randomY = Random.Range(-innerBound.y, innerBound.y);
        Vector3 randomPosition = transform.position + new Vector3(randomX, 0, randomY);
        // Instantiate the enemy at that location
        Instantiate(enemy.enemyPrefab, randomPosition, Quaternion.identity);
    }

#if UNITY_EDITOR
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
        Handles.DrawSolidRectangleWithOutline(new Vector3[] { topLeft, topRight, bottomRight, bottomLeft }, Color.clear, Color.red);

        halfWidth = (boundSize.x - colliderOffset) / 2f;
        halfLength = (boundSize.y - colliderOffset) / 2f;

        // Calculate the four corners of the rect
        topLeft = position + new Vector3(-halfWidth, 0, halfLength);
        topRight = position + new Vector3(halfWidth, 0, halfLength);
        bottomLeft = position + new Vector3(-halfWidth, 0, -halfLength);
        bottomRight = position + new Vector3(halfWidth, 0, -halfLength);

        Handles.DrawSolidRectangleWithOutline(new Vector3[] { topLeft, topRight, bottomRight, bottomLeft }, new Color(1, 0, 0, 0.05f), Color.red);


    }
#endif
}
