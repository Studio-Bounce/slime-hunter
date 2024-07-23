using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_BackToBase : BackToBase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        ((RabbitSlime_FSM)fsm).slimeEnemy.isInvincible = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ((RabbitSlime_FSM)fsm).slimeEnemy.isInvincible = true;
    }
}
