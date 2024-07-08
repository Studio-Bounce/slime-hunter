using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rabbit slime is vulnerable only in cooldown
public class RabbitSlime_Cooldown : BasicSlime_Cooldown
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Make it vulnerable
        fsm.slimeEnemy.isInvincible = false;

        // Eye change to indicate vulnerability
        fsm.slimeEnemy.SetEye(EnemyEye.DEATH);

        // Base handles cooldown timer
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fsm.slimeEnemy.isInvincible = true;
        fsm.slimeEnemy.SetEye(EnemyEye.NORMAL);
    }
}
