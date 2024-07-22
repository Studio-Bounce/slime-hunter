using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] GameObject hitVFXGO;
    [SerializeField] SkinnedMeshRenderer slimeOuterBody;
    [SerializeField] float flashDuration = 0.2f;
    [SerializeField] Material flashMat;
    bool isFlashing = false;

    [Header("Death")]
    [SerializeField] GameObject deathParticlesGO;
    [SerializeField] float deathDelay = 3.5f;

    BasicSlime_FSM fsm;

    protected override void Start()
    {
        base.Start();

        // Ensure that eye game objects are active
        if (normalEye) normalEye.gameObject.SetActive(true);
        if (attackEye) attackEye.gameObject.SetActive(true);
        if (scaredEye) scaredEye.gameObject.SetActive(true);
        if (deathEye) deathEye.gameObject.SetActive(true);
        if (normalEye) normalEye.enabled = true;
        if (attackEye) attackEye.enabled = false;
        if (scaredEye) scaredEye.enabled = false;
        if (deathEye) deathEye.enabled = false;
        isAlive = true;

        fsm = GetComponent<BasicSlime_FSM>();
    }

    // Used in child classes to call the original TakeDamage method
    protected bool BaseEnemyTakeDamage(Damage damage, bool detectDeath)
    {
        bool damageRegistered = base.TakeDamage(damage, detectDeath);
        if (!damageRegistered)
        {
            return false;
        }
        // Returning true because damage was taken but we don't want to act on it
        if (!isAlive)
        {
            return true;
        }

        if (!isInvincible)
        {
            // Hit VFX
            GameObject hitVFXObj = Instantiate(hitVFXGO,
                transform.position + hitVFXGO.transform.position, Quaternion.LookRotation(damage.direction));
            Destroy(hitVFXObj, 2.0f);

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

    IEnumerator FlashSlime()
    {
        isFlashing = true;
        if (slimeOuterBody != null)
        {
            // Change color to white for a short duration
            var newMats = new List<Material>(slimeOuterBody.materials) { flashMat };
            slimeOuterBody.materials = newMats.ToArray();

            yield return new WaitForSeconds(flashDuration);

            // Revert color
            if (slimeOuterBody.materials.Length > 1)
            {
                var oldMats = new List<Material>(slimeOuterBody.materials);
                oldMats.RemoveAt(oldMats.Count - 1);
                slimeOuterBody.materials = oldMats.ToArray();
            }
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
        if (normalEye) normalEye.enabled = (enemyEye == EnemyEye.NORMAL);
        if (attackEye) attackEye.enabled = (enemyEye == EnemyEye.ATTACK);
        if (scaredEye) scaredEye.enabled = enemyEye == EnemyEye.SCARED;
        if (deathEye) deathEye.enabled = (enemyEye == EnemyEye.DEATH);
    }
}
