using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// DamageTaker does not need rigidbody. Its collider triggers the collider of DamageDealer.
// There's no OnTriggerEnter here because of absence of rigidbody.
// As far as DamageTaker is concerned, the collider on it can be trigger or non-trigger. It does not matter
// because in both cases, DamageDealer's OnTriggerEnter will be called.
[RequireComponent(typeof(Collider), typeof(CharacterController), typeof(StatusEffectManager))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public int health = 100;
    protected int maxHealth = 100;
    [HideInInspector, ReadOnly]
    public bool isAlive = true;

    [Tooltip("Damage delay ensures that multiple damages do not get registered in a short interval")]
    public float damageDelay = 0.5f;
    public float knockbackTime = 0.25f;

    protected bool isInKnockback = false;
    // Slime invincibility can depend on Slime's state (in FSM). Hence, its public
    [HideInInspector] public bool isInvincible = false;

    protected CharacterController characterController;
    protected StatusEffectManager statusEffectManager;

    float lastDamageTime = 0;

    // Events
    public UnityEvent onDeathEvent;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        statusEffectManager = GetComponent<StatusEffectManager>();
        maxHealth = health;
    }

    public virtual void Death()
    {
        // Little delay in death prevents bugs from coroutines
        isAlive = false;
        onDeathEvent.Invoke();
        Destroy(gameObject, 0.2f);
    }

    public virtual void TakeDamage(Damage damage)
    {
        // Prevents accidental double damage
        if (Time.time < lastDamageTime + damageDelay)
        {
            return;
        }

        if (!isInvincible)
        {
            health -= (int)damage.value;

            if (damage.effect != null)
            {
                statusEffectManager.AddEffect(damage.effect);
            }
            StartCoroutine(ApplyKnockback(damage));
        }
        lastDamageTime = Time.time;

        if (health <= 0)
        {
            Death();
        }
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
