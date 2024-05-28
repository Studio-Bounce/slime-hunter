using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyEye
{
    NORMAL,
    ATTACK,
    DEATH
};

[RequireComponent(typeof(SphereCollider))]
public class Enemy : DamageTaker, ITakeDamage
{
    [Header("Slime Eyes")]
    [SerializeField] SkinnedMeshRenderer normalEye;
    [SerializeField] SkinnedMeshRenderer attackEye;
    [SerializeField] SkinnedMeshRenderer deathEye;
    private EnemyEye eye;

    [SerializeField] float damageEyeTimer = 1.0f;
    private bool freezeEyeChange = false;
    private TrailRenderer trailRenderer;

    protected override void Start()
    {
        base.Start();
        trailRenderer = GetComponent<TrailRenderer>();
        normalEye.enabled = true;
        attackEye.enabled = false;
        deathEye.enabled = false;
    }

    // Used in child classes to call the original TakeDamage method
    protected void OriginalTakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        StartCoroutine(DisableTrailOnKnockback());
        StartCoroutine(ChangeEyeToDamage());
    }

    IEnumerator DisableTrailOnKnockback()
    {
        float trailTime = 0;
        if (trailRenderer != null)
        {
            trailTime = trailRenderer.time;
            trailRenderer.time = 0;  // to clear trail history
            trailRenderer.enabled = false;
        }
        while (isInKnockback)
        {
            yield return null;
        }
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
            // Wait for a frame to ensure the trail is cleared
            yield return null;
            // Restore the original trail time
            trailRenderer.time = trailTime;
        }
    }

    IEnumerator ChangeEyeToDamage()
    {
        SetEye(EnemyEye.DEATH);
        freezeEyeChange = true;
        yield return new WaitForSeconds(damageEyeTimer);
        freezeEyeChange = false;
        SetEye(EnemyEye.NORMAL);
    }

    public void SetEye(EnemyEye enemyEye)
    {
        StartCoroutine(ChangeEye(enemyEye));
    }

    IEnumerator ChangeEye(EnemyEye enemyEye)
    {
        while (freezeEyeChange)
        {
            yield return null;
        }
        eye = enemyEye;
        normalEye.enabled = (enemyEye == EnemyEye.NORMAL);
        attackEye.enabled = (enemyEye == EnemyEye.ATTACK);
        deathEye.enabled = (enemyEye == EnemyEye.DEATH);
    }

    public EnemyEye GetEnemyEye()
    {
        return eye;
    }
}
