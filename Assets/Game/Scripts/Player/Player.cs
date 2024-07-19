using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
public class Player : DynamicDamageTaker
{
    public float slowDownOnHitMultiplier = 0.2f;
    public float slowDownTime = 0.5f;
    PlayerController playerController;
    WeaponController weaponController;

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        weaponController = GetComponent<WeaponController>();

        // Ensures that base.health does not change as per damage
        GameManager.Instance.PlayerRef = this;
        
        // Don't use DamageTaker's health system
        isInvincible = true;
    }

    public override void Death(bool killObject)
    {
        // Trigger events
        onDeathEvent.Invoke();
    }

    public override bool TakeDamage(Damage damage, bool detectDeath)
    {
        if (playerController.IsDashing ||
            playerController.IsJumping ||
            weaponController.isPerformingSpecialAttack)
            return false;

        // For player, we want to change GameManager.PlayerHealth
        bool damageRegistered = base.TakeDamage(damage, false);
        if (!damageRegistered)
        {
            return false;
        }

        GameManager.Instance.ApplyTempTimeScale(slowDownOnHitMultiplier, slowDownTime);
        GameManager.Instance.PlayerHealth -= (int)damage.value;
        if (GameManager.Instance.PlayerHealth <= 0)
        {
            Death(false);
        }
        return true;
    }
}
