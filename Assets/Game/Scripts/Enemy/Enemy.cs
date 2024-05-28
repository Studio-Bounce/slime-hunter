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
    EnemyEye eye;

    [SerializeField] float damageEyeTimer = 1.0f;
    bool freezeEyeChange = false;
    TrailRenderer trailRenderer;

    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] float hitParticlesDuration = 1.0f;

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
        if (hitParticles != null)
        {
            StartCoroutine(ShowHitParticles());
        }
    }

    public override void TakeDamage(Damage damage)
    {
        base.TakeDamage(damage);
        if (hitParticles != null)
        {
            StartCoroutine(ShowHitParticles());
        }
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

            // Restore the original trail time (Frame has changed so do another null check)
            if (trailRenderer != null)
                trailRenderer.time = trailTime;
        }
    }

    IEnumerator ShowHitParticles()
    {
        if (hitParticles != null)
        {
            hitParticles.Play();
            yield return new WaitForSeconds(hitParticlesDuration);
            hitParticles.Stop();
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
