using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For the objects which can take damage and move.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(StatusEffectManager))]
public class DynamicDamageTaker : UIDamageTaker
{
    public float knockbackTime = 0.25f;

    protected bool isInKnockback = false;

    protected CharacterController characterController;
    protected StatusEffectManager statusEffectManager;

    public List<StatusEffect> activeEffects { get { return statusEffectManager.activeEffects; } }

    protected override void Start()
    {
        base.Start();

        characterController = GetComponent<CharacterController>();
        statusEffectManager = GetComponent<StatusEffectManager>();
    }

    public override bool TakeDamage(Damage damage, bool detectDeath)
    {
        bool damageRegistered = base.TakeDamage(damage, detectDeath);
        if (!damageRegistered)
        {
            return false;
        }

        if (!isInvincible || damage.forceApply)
        {
            if (damage.effect != null)
            {
                statusEffectManager.AddEffect(damage.effect);
                StartCoroutine(ApplyKnockback(damage));
            }
        }
        return true;
    }


    protected IEnumerator ApplyKnockback(Damage damage)
    {
        isInKnockback = true;
        Vector3 knockbackVec = damage.direction * damage.knockback * GameManager.GLOBAL_KNOCKBACK_MULTIPLIER;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + knockbackVec;
        float timeElapsed = 0.0f;
        while (timeElapsed < knockbackTime)
        {
            // Lerp knockback
            float normalizedTime = timeElapsed / knockbackTime;
            float t = Easing.EaseOutCubic(normalizedTime);
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);

            if (characterController != null && characterController.enabled)
                characterController.Move(newPosition - transform.position);
            else
                transform.Translate(newPosition - transform.position);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isInKnockback = false;
    }
}
