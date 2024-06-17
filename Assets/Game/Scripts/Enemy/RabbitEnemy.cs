using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitEnemy : Enemy
{
    public readonly int DodgeTrigger = Animator.StringToHash("dodge");
    public readonly int DodgeSpeedField = Animator.StringToHash("dodgeSpeed");

    [Header("Slime Dodge")]
    [SerializeField] float dodgeDistance = 5.0f;
    [SerializeField] float dodgeTime = 1.0f;

    [SerializeField] Animator slimeAnimator;

    RabbitSlime_FSM rfsm;
    Trail slimeTrail;

    // Used by FSM states
    [HideInInspector] public bool isDodging = false;

    protected override void Start()
    {
        base.Start();

        slimeTrail = GetComponent<Trail>();
        slimeTrail.activeTime = Mathf.Max(0.0f, dodgeTime - 0.1f);
        rfsm = GetComponent<RabbitSlime_FSM>();
    }

    public override void TakeDamage(Damage damage)
    {
        base.BaseEnemyTakeDamage(damage);

        // Rabbit slime can not be stopped when its actively attacking
        bool isAttackStoppable = (rfsm.GetAttackState() == BasicSlime_AttackPlayer.SlimeAttackState.CHARGE_UP ||
                                  rfsm.GetAttackState() == BasicSlime_AttackPlayer.SlimeAttackState.NONE);
        if (isInvincible && isAlive && isAttackStoppable)
        {
            // Dodge
            // HACK: FSM state change happening outside of actual FSM (fixme)
            rfsm.ChangeState(rfsm.DodgeStateName);
        }
    }

    public void Dodge()
    {
        slimeAnimator.SetTrigger(DodgeTrigger);
        // Dodge animation length is 1 seconds, so the scaling works perfectly
        slimeAnimator.SetFloat(DodgeSpeedField, (1 / dodgeTime));

        isDodging = true;
        Vector3 playerDirection = rfsm.GetPlayerPosition() - transform.position;
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
        Vector3 playerToSlime = transform.position - rfsm.GetPlayerPosition();
        playerToSlime.Normalize();
        dodgeDirection += playerToSlime;
        dodgeDirection.Normalize();

        StartCoroutine(ApplyDodge(dodgeDirection * dodgeDistance));
    }

    IEnumerator ApplyDodge(Vector3 dodgeVec)
    {
        slimeTrail.InitiateTrail();
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + dodgeVec;
        float timeElapsed = 0.0f;
        while (timeElapsed < dodgeTime)
        {
            // Lerp knockback
            float t = Easing.EaseOutCubic(timeElapsed / dodgeTime);
            Vector3 newPosition = Vector3.Lerp(startPosition, endPosition, t);
            if (characterController != null && characterController.enabled)
                characterController.Move(newPosition - transform.position);
            else
                transform.Translate(newPosition - transform.position);

            timeElapsed += Time.deltaTime;
            yield return null;
        }
        isDodging = false;
    }

}
