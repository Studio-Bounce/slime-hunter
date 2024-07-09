using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct EnemyNestItem
{
    public GameObject enemyPrefab;
    public int enemyCount;
    public float spawnDelay;
}

public class EnemyNest : MonoBehaviour
{
    [SerializeField] List<EnemyNestItem> enemyNestItems = new List<EnemyNestItem>();
    [Tooltip("Amount of time delay between different enemy nest items.")]
    [SerializeField] float waveDelay = 2.0f;
    [Tooltip("Enemies are spawned this far from the border")]
    [SerializeField] float borderBuffer = 1.0f;

    bool isSpawning = false;

    public void ActivateNest()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnEnemies());
        }
    }

    public void DeactivateNest()
    {
        isSpawning = false;
    }

    IEnumerator SpawnEnemies()
    {
        int enemyNestIdx = 0;
        float scaleX = transform.localScale.x, scaleZ = transform.localScale.z;
        while (isSpawning)
        {
            EnemyNestItem enemyNest = enemyNestItems[enemyNestIdx];
            for (int i = 0; i < enemyNest.enemyCount; i++)
            {
                // Random position in one of the 4 sides
                float deltaX = 0, deltaZ = 0;
                if (Random.Range(0f, 1f) < 0.5f)
                {
                    deltaX = Random.Range((-scaleX / 2f) - borderBuffer, (scaleX / 2) + borderBuffer);
                    deltaZ = (Random.Range(0f, 1f) < 0.5f) ? ((-scaleZ / 2f) - borderBuffer) : ((scaleZ / 2) + borderBuffer);
                }
                else
                {
                    deltaX = (Random.Range(0f, 1f) < 0.5f) ? ((-scaleX / 2f) - borderBuffer) : ((scaleX / 2) + borderBuffer);
                    deltaZ = Random.Range((-scaleZ / 2f) - borderBuffer, (scaleZ / 2) + borderBuffer);
                }
                Vector3 enemyPos = transform.position + new Vector3(deltaX, 0, deltaZ);

                // TODO: Spawn in correct scene
                Instantiate(enemyNest.enemyPrefab, enemyPos, Quaternion.identity);

                yield return new WaitForSeconds(enemyNest.spawnDelay);
            }
            enemyNestIdx = (enemyNestIdx + 1) % enemyNestItems.Count;

            yield return new WaitForSeconds(waveDelay);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Spawn zone
        float scaleX = transform.localScale.x, scaleZ = transform.localScale.z;
        float minDeltaX = (-scaleX / 2f) - borderBuffer;
        float maxDeltaX = (scaleX / 2) + borderBuffer;
        float minDeltaZ = (-scaleZ / 2f) - borderBuffer;
        float maxDeltaZ = (scaleZ / 2) + borderBuffer;

        Vector3 pointA = transform.position + new Vector3(minDeltaX, 0, minDeltaZ);
        Vector3 pointB = transform.position + new Vector3(minDeltaX, 0, maxDeltaZ);
        Vector3 pointC = transform.position + new Vector3(maxDeltaX, 0, maxDeltaZ);
        Vector3 pointD = transform.position + new Vector3(maxDeltaX, 0, minDeltaZ);
        Debug.DrawLine(pointA, pointB, Color.red);
        Debug.DrawLine(pointB, pointC, Color.red);
        Debug.DrawLine(pointC, pointD, Color.red);
        Debug.DrawLine(pointD, pointA, Color.red);
    }
#endif
}
