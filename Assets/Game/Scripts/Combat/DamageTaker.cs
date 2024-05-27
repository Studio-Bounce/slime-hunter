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
    [SerializeField] Slider slider;
    private int maxHealth = 0;

    [Tooltip("Damage delay ensures that multiple damages do not get registered in a short interval")]
    public float damageDelay = 0.5f;
    public float knockbackTime = 0.25f;

    float lastDamageTime = 0;
    CharacterController characterController;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
        maxHealth = health;
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public virtual void TakeDamage(Damage damage)
    {
        // Prevents accidental double damage
        if (Time.time < lastDamageTime + damageDelay)
        {
            return;
        }

        health -= damage.value;
        StartCoroutine(ApplyKnockback(damage));
        lastDamageTime = Time.time;
        if (slider != null)
        {
            slider.value = ((float) health / maxHealth);
        }

        if (health <= 0)
        {
            Death();
        }
    }

    private IEnumerator ApplyKnockback(Damage damage)
    {
        Vector3 knockbackVec = damage.direction * damage.knockback;
        knockbackVec.y = 0;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + knockbackVec;
        float timeElapsed = 0.0f;
        while (timeElapsed < knockbackTime)
        {
            // Lerp knockback
            float t = EasingFunctions.EaseOutCubic(timeElapsed / knockbackTime);
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);
            if (characterController != null && characterController.enabled)
                characterController.Move(newPosition - transform.position);
            else
                transform.Translate(newPosition - transform.position);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }
}
