using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rabbit slime sneaks up on player
// Its target is behind the player
public class RabbitSlime_ChasePlayer : BasicSlime_ChasePlayer
{
    public override void SetChaseTarget()
    {
        // .25f buffer to ensure chase target lies within attack radius
        fsm.seekSteeringBehaviour.target = fsm.GetPlayerPosition() - ((fsm.attackRadius - 0.25f) * fsm.GetPlayerForward());
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        // Move fast
        ((RabbitSlime_FSM)fsm).SetChaseAnimationSpeed();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        ((RabbitSlime_FSM)fsm).ResetMoveAnimationSpeed();
    }
}
