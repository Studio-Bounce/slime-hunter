using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// DamageTaker does not need rigidbody. Its collider triggers the collider of DamageDealer.
// There's no OnTriggerEnter here because of absence of rigidbody.
// As far as DamageTaker is concerned, the collider on it can be trigger or non-trigger. It does not matter
// because in both cases, DamageDealer's OnTriggerEnter will be called.
[RequireComponent(typeof(Collider))]
public class DamageTaker : MonoBehaviour, ITakeDamage
{
    public int health = 100;
    protected int maxHealth = 100;
    [HideInInspector, ReadOnly]
    public bool isAlive = true;

    [Tooltip("Damage delay ensures that multiple damages do not get registered in a short interval")]
    public float damageDelay = 0.5f;

    // Slime invincibility can depend on Slime's state (in FSM). Hence, its public
    [HideInInspector] public bool isInvincible = false;

    float lastDamageTime = 0;

    // Events
    public UnityEvent onDeathEvent;
    public UnityEvent onHitEvent;

    protected virtual void Start()
    {
        maxHealth = health;
    }

    public virtual void Death(bool killObject)
    {
        isAlive = false;
        onDeathEvent.Invoke();
        if (killObject)
        {
            // Little delay in death prevents bugs from coroutines
            Destroy(gameObject, 0.2f);
        }
    }

    public virtual bool TakeDamage(Damage damage, bool detectDeath)
    {
        // Prevents accidental double damage
        if (Time.time < lastDamageTime + damageDelay/GameManager.Instance.PlayerSpeedMultiplier)
        {
            return false;
        }
        lastDamageTime = Time.time;

        if (!isInvincible || damage.forceApply)
        {
            onHitEvent.Invoke();
            health -= (int)damage.value;
        }

        if (detectDeath && health <= 0)
        {
            Death(true);
        }
        return true;
    }
}
