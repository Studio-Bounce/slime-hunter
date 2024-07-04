using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberSlime_FSM : BasicSlime_FSM
{
    [Header("Bomber Slime Attributes")]
    public float damageRadius = 5.0f;

    [HideInInspector] public EnemyBomb enemyBomb;
    bool didExplode = false;

    protected override void Start()
    {
        base.Start();

        didExplode = false;
        enemyBomb = GetComponentInChildren<EnemyBomb>();
        enemyBomb.damageRadius = damageRadius;
    }

    public void Explode()
    {
        if (!didExplode)
        {
            didExplode = true;
            Emit(10);
            StartCoroutine(ExplosionSequence());
        }
    }

    IEnumerator ExplosionSequence()
    {
        // wait for glow
        yield return new WaitForSeconds(attackEmissionTime);

        enemyBomb.Explode();
    }

    private void OnValidate()
    {
        // Nullify the fields which are not used for bomber slime
        cooldownTime = 0;
        attackSpeed = 0;
        attackProximity = 0;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        DebugExtension.DrawCircle(transform.position, Color.black, damageRadius);
    }
}
