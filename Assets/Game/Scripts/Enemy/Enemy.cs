using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.VFX;

public enum EnemyEye
{
    NORMAL,
    ATTACK,
    SCARED,
    DEATH
};

[RequireComponent(typeof(SphereCollider))]
public class Enemy : DynamicDamageTaker
{
    [Header("Slime Eyes")]
    [SerializeField] SkinnedMeshRenderer normalEye;
    [SerializeField] SkinnedMeshRenderer attackEye;
    [SerializeField] SkinnedMeshRenderer scaredEye;
    [SerializeField] SkinnedMeshRenderer deathEye;
    EnemyEye eye;

    [SerializeField] float damageEyeTimer = 1.0f;
    bool freezeEyeChange = false;

    [Header("Hit Feedback")]
    [SerializeField] VisualEffect hitVFX;
    [SerializeField] float hitVFXDuration = 1.0f;
    [SerializeField] SkinnedMeshRenderer slimeOuterBody;
    [SerializeField] float flashDuration = 0.2f;
    [SerializeField][ColorUsage(true, true)] Color flashColor;
    bool hitVFXPlaying = false;
    bool isFlashing = false;

    [Header("Death")]
    [SerializeField] GameObject slimeModel;
    [SerializeField] GameObject deathParticlesGO;
    [SerializeField] float deathDelay = 3.5f;

    BasicSlime_FSM fsm;

    protected override void Start()
    {
        base.Start();

        // Ensure that eye game objects are active
        normalEye?.gameObject.SetActive(true);
        attackEye?.gameObject.SetActive(true);
        if (scaredEye) scaredEye.gameObject.SetActive(true);
        deathEye?.gameObject.SetActive(true);
        normalEye.enabled = true;
        attackEye.enabled = false;
        if (scaredEye) scaredEye.enabled = false;
        deathEye.enabled = false;
        isAlive = true;

        fsm = GetComponent<BasicSlime_FSM>();
    }

    // Used in child classes to call the original TakeDamage method
    protected bool BaseEnemyTakeDamage(Damage damage, bool detectDeath)
    {
        bool damageRegistered = base.TakeDamage(damage, detectDeath);
        if (!isAlive || !damageRegistered)
        {
            return false;
        }

        if (!isInvincible)
        {
            if (!hitVFXPlaying)
                StartCoroutine(ShowHitParticles());
            if (!isFlashing)
                StartCoroutine(FlashSlime());
        }
        return true;
    }

    public override bool TakeDamage(Damage damage, bool detectDeath)
    {
        bool damageRegistered = BaseEnemyTakeDamage(damage, true);
        if (!damageRegistered)
        {
            return false;
        }

        if (!isInvincible && !freezeEyeChange)
        {
            StartCoroutine(ChangeEyeToDamage());
        }
        return true;
    }

    public override void Death(bool killObject)
    {
        // Don't destroy the object yet
        base.Death(false);

        fsm.ChangeState(fsm.DeadStateName);

        // Death particles
        GameObject deathObj = Instantiate(deathParticlesGO, transform.position, Quaternion.identity);
        deathObj.GetComponent<ParticleSystem>().Play();
        Destroy(deathObj, deathDelay);

        // Time to die
        Destroy(gameObject);
    }

    public void SetEye(EnemyEye enemyEye)
    {
        StartCoroutine(ChangeEye(enemyEye));
    }

    public EnemyEye GetEnemyEye()
    {
        return eye;
    }

    IEnumerator ShowHitParticles()
    {
        hitVFXPlaying = true;
        if (hitVFX != null)
        {
            hitVFX.Play();
            yield return new WaitForSeconds(hitVFXDuration);
            hitVFX.Stop();
        }
        hitVFXPlaying = false;
    }

    IEnumerator FlashSlime()
    {
        isFlashing = true;
        if (slimeOuterBody != null)
        {
            Color slimeColor = slimeOuterBody.materials[0].color;

            // Change color to white for a short duration
            slimeOuterBody.materials[0].color = flashColor;

            yield return new WaitForSeconds(flashDuration);

            // Revert color
            slimeOuterBody.materials[0].color = slimeColor;
        }
        isFlashing = false;
    }

    IEnumerator ChangeEyeToDamage()
    {
        freezeEyeChange = true;
        SetEye(EnemyEye.DEATH);
        yield return new WaitForSeconds(damageEyeTimer);
        SetEye(EnemyEye.NORMAL);
        freezeEyeChange = false;
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
        if (scaredEye) scaredEye.enabled = enemyEye == EnemyEye.SCARED;
        deathEye.enabled = (enemyEye == EnemyEye.DEATH);
    }
}
