using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CapsuleCollider))]
public class Player : DamageTaker
{
    public float slowDownOnHitMultiplier = 0.2f;
    public int slowDownOnHitFrames = 5;
    PlayerController playerController;

    private void Awake()
    {
        GameManager.Instance.playerRef = this;
    }

    protected override void Start()
    {
        base.Start();
        // Ensures that base.health does not change as per damage
        isInvincible = true;

        playerController = GetComponent<PlayerController>();
    }

    public override void Death()
    {
        // Trigger events
        onDeathEvent.Invoke();
        GameManager.Instance.PlayerHealth = GameManager.Instance.PlayerMaxHealth;
    }

    public override void TakeDamage(Damage damage)
    {
        // Can not take damage if dashing or jumping
        if (playerController.IsDashing() || playerController.IsJumping())
            return;

        // For player, we want to change GameManager.PlayerHealth
        base.TakeDamage(damage);

        StartCoroutine(SlowDownOnHit());
        GameManager.Instance.PlayerHealth -= (int)damage.value;
        if (GameManager.Instance.PlayerHealth <= 0)
        {
            Death();
        }
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
