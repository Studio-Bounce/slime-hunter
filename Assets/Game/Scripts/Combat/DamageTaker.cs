using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// DamageTaker does not need rigidbody. Its collider triggers the collider of DamageDealer.
// There's no OnTriggerEnter here because of absence of rigidbody.
// As far as DamageTaker is concerned, the collider on it can be trigger or non-trigger. It does not matter
// because in both cases, DamageDealer's OnTriggerEnter will be called.
[RequireComponent(typeof(Collider))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public int health = 100;
    [SerializeField] protected Slider healthSlider;
    private int maxHealth = 0;

    [Tooltip("Damage delay ensures that multiple damages do not get registered in a short interval")]
    public float damageDelay = 0.5f;
    public float knockbackTime = 0.25f;

    protected CharacterController characterController;
    protected bool isInKnockback = false;
    // Slime invincibility can depend on Slime's state (in FSM). Hence, its public
    [HideInInspector] public bool isInvincible = false;

    float lastDamageTime = 0;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        maxHealth = health;
    }

    public virtual void Death()
    {
        // Little delay in death prevents bugs from coroutines
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
            health -= damage.value;
            StartCoroutine(ApplyKnockback(damage));
        }
        lastDamageTime = Time.time;
        if (healthSlider != null)
        {
            healthSlider.value = ((float) health / maxHealth);
        }

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
            float t = EasingFunctions.EaseOutCubic(normalizedTime);
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);
            //float lerpedY = Mathf.Sin(normalizedTime * Mathf.PI)*Mathf.Log(damage.knockback);
            //newPosition = new Vector3(newPosition.x, lerpedY, newPosition.z);

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
