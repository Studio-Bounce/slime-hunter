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

    [Header("Hit Feedback")]
    [SerializeField] ParticleSystem hitParticles;
    [SerializeField] float hitParticlesDuration = 1.0f;
    [SerializeField] SkinnedMeshRenderer slimeOuterBody;
    [SerializeField] float flashDuration = 0.2f;
    [SerializeField] Color flashColor;

    [Header("Death")]
    [SerializeField] GameObject slimeModel;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] float deathDelay = 3.5f;
    protected bool isAlive = true;

    protected override void Start()
    {
        base.Start();
        trailRenderer = GetComponent<TrailRenderer>();
        normalEye.enabled = true;
        attackEye.enabled = false;
        deathEye.enabled = false;
        isAlive = true;
    }

    // Used in child classes to call the original TakeDamage method
    protected void BaseEnemyTakeDamage(Damage damage)
    {
        if (!isAlive)
        {
            return;
        }
        base.TakeDamage(damage);
        if (!isInvincible)
        {
            StartCoroutine(ShowHitParticles());
            StartCoroutine(FlashSlime());
        }
    }

    public override void TakeDamage(Damage damage)
    {
        BaseEnemyTakeDamage(damage);
        if (!isInvincible)
        {
            //StartCoroutine(DisableTrailOnKnockback());
            StartCoroutine(ChangeEyeToDamage());
        }
    }

    public override void Death()
    {
        isAlive = false;

        // Hide visible meshes / UI
        slimeModel.SetActive(false);
        healthSlider.gameObject.SetActive(false);

        // Stop movement
        GetComponent<SlimeSteeringAgent>().enabled = false;

        deathParticles.Play();
        Destroy(gameObject, deathDelay);
    }

    public void SetEye(EnemyEye enemyEye)
    {
        StartCoroutine(ChangeEye(enemyEye));
    }

    public EnemyEye GetEnemyEye()
    {
        return eye;
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
            // Check if the GameObject has been destroyed
            if (trailRenderer == null || trailRenderer.gameObject == null)
            {
                yield break;
            }
            yield return null;
        }
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;

            // Wait for a frame to ensure the trail is cleared
            yield return null;

            // Restore the original trail time (Frame has changed so do another null check)
            if (trailRenderer != null && trailRenderer.gameObject != null)
            {
                trailRenderer.time = trailTime;
            }
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

    IEnumerator FlashSlime()
    {
        if (slimeOuterBody != null)
        {
            Color slimeColor = slimeOuterBody.materials[0].color;

            // Change color to white for a short duration
            slimeOuterBody.materials[0].color = flashColor;

            yield return new WaitForSeconds(flashDuration);

            // Revert color
            slimeOuterBody.materials[0].color = slimeColor;
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
}
