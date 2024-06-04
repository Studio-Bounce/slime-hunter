using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Player : DamageTaker
{
    private void Awake()
    {
        GameManager.Instance.playerRef = this;
    }

    protected override void Start()
    {
        base.Start();
        // Ensures that base.health does not change as per damage
        isInvincible = true;
    }

    public override void Death()
    {
        // Trigger events
        onDeathEvent.Invoke();
        GameManager.Instance.PlayerHealth = GameManager.Instance.PlayerMaxHealth;
    }

    public override void TakeDamage(Damage damage)
    {
        // For player, we want to change GameManager.PlayerHealth
        base.TakeDamage(damage);

        GameManager.Instance.PlayerHealth -= (int)damage.value;
        if (GameManager.Instance.PlayerHealth <= 0)
        {
            Death();
        }
    }
}
