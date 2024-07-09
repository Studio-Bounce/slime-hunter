using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
public class Player : DynamicDamageTaker
{
    public float slowDownOnHitMultiplier = 0.2f;
    public int slowDownOnHitFrames = 5;
    PlayerController playerController;

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();

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
        // Can not take damage if dashing or jumping
        if (playerController.IsDashing || playerController.IsJumping)
            return false;

        // For player, we want to change GameManager.PlayerHealth
        bool damageRegistered = base.TakeDamage(damage, false);
        if (!damageRegistered)
        {
            return false;
        }

        StartCoroutine(SlowDownOnHit());
        GameManager.Instance.PlayerHealth -= (int)damage.value;
        if (GameManager.Instance.PlayerHealth <= 0)
        {
            Death(false);
        }
        return true;
    }

    IEnumerator SlowDownOnHit()
    {
        Time.timeScale = slowDownOnHitMultiplier;
        for (int i = 0; i < slowDownOnHitFrames; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        Time.timeScale = 1;
    }
}
