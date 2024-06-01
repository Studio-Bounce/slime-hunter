using System.Collections;
using System.Collections.Generic;
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
public struct EnemyWave
{
    public List<EnemySpawnProperties> enemiesInWave;

    public int totalEnemies;
    public float spawnInterval;
    public float delayOnComplete;

    public EnemySpawnProperties RollEnemy()
    {
        // Calculate the total weight
        float totalWeight = 0f;
        foreach (var enemy in enemiesInWave)
        {
            totalWeight += enemy.spawnWeight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        // Determine which enemy to spawn
        float cumulativeWeight = 0f;
        foreach (var enemy in enemiesInWave)
        {
            cumulativeWeight += enemy.spawnWeight;
            if (randomValue <= cumulativeWeight)
            {
                return enemy;
            }
        }
        return enemiesInWave[0];
    }
}