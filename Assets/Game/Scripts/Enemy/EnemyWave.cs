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
    public float spawnWeight; // A value between 0 and 1
    public SpawnLocation spawnLocation;
}

[System.Serializable]

public class EnemyWaveProperties
{
    public List<EnemySpawnProperties> enemiesInWave;
    public int totalEnemies;
    public float spawnInterval;
    public float delayOnComplete;
}

public class EnemyWave : MonoBehaviour
{
    private EnemyWaveProperties properties;
    private float _totalWeight;
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private bool isStart = false;
    private float timer = 0;
    private int enemySpawnCount = 0;
    private Vector2 bounds;
    private bool isComplete = false;
    private int totalDeaths = 0;

    public bool Completed { get { return isComplete; } }
    public bool Started { get { return isStart; } }

    // Using timer method as coroutines have issue instantiating FSM
    public void Update()
    {
        if (!Started) return;

        timer += Time.deltaTime;
        if (enemySpawnCount < properties.totalEnemies)
        {
            // Exit early if not time to spawn
            if (!Utils.CheckTimer(ref timer, properties.spawnInterval)) return;

            EnemySpawnProperties enemy = RollEnemy();
            switch (enemy.spawnLocation)
            {
                case SpawnLocation.RANDOM_BORDER:
                    SpawnAtBorder(enemy.enemyPrefab);
                    break;
                case SpawnLocation.RANDOM_INNER:
                    SpawnAtInner(enemy.enemyPrefab);
                    break;
                default:
                    break;
            }
            enemySpawnCount++;
        } else
        {
            // Don't complete unless all enemies killed
            if (totalDeaths < properties.totalEnemies) return;

            if (Utils.CheckTimer(ref timer, properties.delayOnComplete))
            {
                isComplete = true;
            }
        }
    }

    public void Reset()
    {
        properties = null;
        bounds = Vector2.zero;
        spawnedEnemies.Clear();
        timer = 0;
        enemySpawnCount = 0;
        totalDeaths = 0;
        isStart = false;
        isComplete = false;
    }

    public void StartWave(EnemyWaveProperties _properties, Vector2 _bounds)
    {
        properties = _properties;
        _totalWeight = 0f;
        foreach (var enemy in properties.enemiesInWave)
        {
            _totalWeight += enemy.spawnWeight;
        }
        bounds = _bounds;
        isStart = true;
    }

    public EnemySpawnProperties RollEnemy()
    {
        float randomValue = Random.Range(0f, _totalWeight);
        // Determine which enemy to spawn
        float cumulativeWeight = 0f;
        foreach (var enemy in properties.enemiesInWave)
        {
            cumulativeWeight += enemy.spawnWeight;
            if (randomValue <= cumulativeWeight)
            {
                return enemy;
            }
        }
        return properties.enemiesInWave[0];
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
    }
}