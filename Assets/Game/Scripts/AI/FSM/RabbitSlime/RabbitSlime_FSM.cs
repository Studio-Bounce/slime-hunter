using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_FSM : BasicSlime_FSM
{
    public readonly int DodgeStateName = Animator.StringToHash("Dodge");

    [Tooltip("It will stay for this time in Idle state before switching to Move")]
    [SerializeField] float idleTime = 2.0f;
    [Tooltip("It will stay for this time in Move state before switching to Idle")]
    [SerializeField] float moveTime = 5.0f;

    public bool IsResting { get; private set; }
    public bool ResetRestOnNextUpdate { get; set; }

    protected override void Start()
    {
        base.Start();
        // Rabbit slime is invincible at the beginning
        slimeEnemy.isInvincible = true;

        IsResting = true;
        StartCoroutine(SwitchBetweenRestAndMotion());
    }

    IEnumerator SwitchBetweenRestAndMotion()
    {
        float timeElapsed = 0.0f;
        while (true)
        {
            if (ResetRestOnNextUpdate)
            {
                // Rabbit just entered wander state
                ResetRestOnNextUpdate = false;
                timeElapsed = 0.0f;
            }

            IsResting = (timeElapsed <= idleTime);
            timeElapsed = (timeElapsed + Time.deltaTime) % (idleTime + moveTime);

            yield return null;
        }
    }

}
