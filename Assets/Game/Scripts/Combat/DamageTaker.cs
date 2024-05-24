using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DamageTaker does not need rigidbody. Its collider triggers the collider of DamageDealer.
// There's no OnTriggerEnter here because of absence of rigidbody.
// As far as DamageTaker is concerned, the collider on it can be trigger or non-trigger. It does not matter
// because in both cases, DamageDealer's OnTriggerEnter will be called.
[RequireComponent(typeof(Collider))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public int health = 100;

    CharacterController characterController;

    protected virtual void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    public virtual void TakeDamage(Damage damage)
    {
        health -= damage.value;
        StartCoroutine(ApplyKnockback(damage));

        if (health <= 0)
        {
            Death();
        }
    }

    // DamageTaker does not have RigidBody. So, knockback uses transform translate.
    private IEnumerator ApplyKnockback(Damage damage)
    {
        Vector3 knockbackVec = damage.direction * damage.knockback;
        knockbackVec.y = 0;
        if (characterController != null && characterController.enabled)
            characterController.Move(knockbackVec);
        else
            transform.Translate(knockbackVec);
        yield return null;
    }
}
