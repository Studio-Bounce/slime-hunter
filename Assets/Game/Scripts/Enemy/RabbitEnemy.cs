using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitEnemy : Enemy
{
    [Header("Slime Dodge")]
    [SerializeField] float dodgeSpeed = 5.0f;
    [SerializeField] float dodgeTime = 1.0f;

    RabbitSlime_FSM fsm;
    Transform playerTransform;
    TrailRenderer trailRenderer;

    // Used by FSM states
    [HideInInspector] public bool isDodging = false;

    protected override void Start()
    {
        base.Start();
        trailRenderer = GetComponent<TrailRenderer>();
        fsm = GetComponent<RabbitSlime_FSM>();
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        UnityEngine.Assertions.Assert.IsNotNull(playerTransform, "GameObject with tag 'Player' not found!");
    }

    public override void TakeDamage(Damage damage)
    {
        base.BaseEnemyTakeDamage(damage);

        if (isInvincible && isAlive)
        {
            // Dodge
            // HACK: FSM state change happening outside of actual FSM (fixme)
            fsm.ChangeState(fsm.DodgeStateName);
        }
    }

    public void Dodge()
    {
        isDodging = true;
        Vector3 playerDirection = playerTransform.position - transform.position;
        playerDirection.Normalize();
        
        // Dodge in a direction orthogonal to the slime-player vector
        Vector3 dodgeDirection;
        if (Random.Range(0, 10) < 5)
        {
            dodgeDirection = new Vector3(-playerDirection.z, playerDirection.y, playerDirection.x);
        }
        else
        {
            dodgeDirection = new Vector3(playerDirection.z, playerDirection.y, -playerDirection.x);
        }
        // Add player-to-slime vector to dodge slime away from player
        Vector3 playerToSlime = transform.position - playerTransform.position;
        playerToSlime.Normalize();
        dodgeDirection += playerToSlime;
        dodgeDirection.Normalize();

        StartCoroutine(ApplyDodge(dodgeDirection * dodgeSpeed));
    }

    IEnumerator ApplyDodge(Vector3 dodgeDirection)
    {
        trailRenderer.enabled = true;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + dodgeDirection;
        float timeElapsed = 0.0f;
        while (timeElapsed < dodgeTime)
        {
            // Lerp knockback
            float t = EasingFunctions.EaseOutQuart(timeElapsed / dodgeTime);
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);
            if (characterController != null && characterController.enabled)
                characterController.Move(newPosition - transform.position);
            else
                transform.Translate(newPosition - transform.position);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
        trailRenderer.enabled = false;
    }

}
