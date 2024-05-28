using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Player : DamageTaker
{
    protected override void Start()
    {
        base.Start();

        // Ensures that base.health does not change as per damage
        isInvincible = true;
    }

    public override void TakeDamage(Damage damage)
    {
        // For player, we want to change GameManager.PlayerHealth
        base.TakeDamage(damage);

        GameManager.Instance.PlayerHealth -= damage.value;
        StartCoroutine(ApplyKnockback(damage));
    }
}
