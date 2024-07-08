using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SpawnLocation
{
    RANDOM_BORDER,
    RANDOM_INNER
}

[System.Serializable]
public struct EnemySpawnProperties
{
    public GameObject enemyPrefab;
    public int spawnCount;
    public SpawnLocation spawnLocation;
}

[System.Serializable]

public class EnemyWaveProperties
{
    public List<EnemySpawnProperties> enemiesInWave;
    public float delayOnComplete;
}

public class EnemyWave : MonoBehaviour
{
    private EnemyWaveProperties properties;
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private bool isStart = false;
    private float timer = 0;
    private int totalEnemies = 0;
    private Vector2 bounds;
    private bool isComplete = false;
    public int totalDeaths = 0;

    public bool Completed { get { return isComplete; } }
    public bool Started { get { return isStart; } }

    // Using timer method as coroutines have issue instantiating FSM
    private void Update()
    {
        if (!Started) return;

        // Don't complete unless all enemies killed
        if (totalDeaths < totalEnemies) return;

        timer += Time.deltaTime;
        if (Utils.CheckTimer(ref timer, properties.delayOnComplete))
        {
            isComplete = true;
        }
    }

    public void ResetWaves(bool cleanSlimes)
    {
        properties = null;
        bounds = Vector2.zero;
        timer = 0;
        totalEnemies = 0;
        isStart = false;
        isComplete = false;
        // Kill the slimes spawned in an enemy wave
        if (cleanSlimes)
        {
            foreach (Enemy enemy in spawnedEnemies)
            {
                if (enemy != null && enemy.gameObject != null)
                {
                    enemy.Death();
                }
            }
        }
        totalDeaths = 0;  // Enemy death increments the counter, so reset it afterwards
        spawnedEnemies.Clear();
    }

    public void StartWave(EnemyWaveProperties _properties, Vector2 _bounds)
    {
        properties = _properties;
        bounds = _bounds;
        isStart = true;

        foreach (EnemySpawnProperties prop in properties.enemiesInWave)
        {
            totalEnemies += prop.spawnCount;
        }

        foreach (EnemySpawnProperties prop in properties.enemiesInWave)
        {
            for (int i = 0; i < prop.spawnCount; i++)
            {
                switch (prop.spawnLocation)
                {
                    case SpawnLocation.RANDOM_BORDER:
                        SpawnAtBorder(prop.enemyPrefab);
                        break;
                    case SpawnLocation.RANDOM_INNER:
                        SpawnAtInner(prop.enemyPrefab);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void SpawnAtBorder(GameObject enemy)
    {
        // Select a random border location
        int border = Random.Range(0, 4); // 0 = top, 1 = right, 2 = bottom, 3 = left
        float x = 0f;
        float y = 0f;
        switch (border)
        {
            case 0: // Top border
                x = Random.Range(-bounds.x, bounds.x);
                y = bounds.y;
                break;
            case 1: // Right border
                x = bounds.x;
                y = Random.Range(-bounds.y, bounds.y);
                break;
            case 2: // Bottom border
                x = Random.Range(-bounds.x, bounds.x);
                y = -bounds.y;
                break;
            case 3: // Left border
                x = -bounds.x;
                y = Random.Range(-bounds.y, bounds.y);
                break;
        }
        Vector3 borderPosition = transform.position + new Vector3(x, 0, y);
        // Instantiate the object at the border position
        Enemy enemyRef = Instantiate(enemy, borderPosition, Quaternion.identity, transform).GetComponent<Enemy>();
        enemyRef.onDeathEvent.AddListener(() => totalDeaths++);

        spawnedEnemies.Add(enemyRef);
    }

    private void SpawnAtInner(GameObject enemy)
    {
        // Generate a random position within the specified area
        float randomX = Random.Range(-bounds.x, bounds.x);
        float randomY = Random.Range(-bounds.y, bounds.y);
        Vector3 randomPosition = transform.position + new Vector3(randomX, 0, randomY);
        // Instantiate the enemy at that location
        Enemy enemyRef = Instantiate(enemy, randomPosition, Quaternion.identity, transform).GetComponent<Enemy>();
        enemyRef.onDeathEvent.AddListener(() => totalDeaths++);
        spawnedEnemies.Add(enemyRef);
    }
}