using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberEnemy : Enemy
{
    EnemyBomb enemyBomb;

    protected override void Start()
    {
        base.Start();

        enemyBomb = GetComponentInChildren<EnemyBomb>();
    }

    public override void TakeDamage(Damage damage)
    {
        BaseEnemyTakeDamage(damage);

        // Explode
        enemyBomb.Explode();
    }
}
