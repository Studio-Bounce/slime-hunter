using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_Dodge : BasicSlime_BaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop the agent
        fsm.slimeAgent.reachedGoal = true;
        fsm.slimeAgent.velocity = Vector3.zero;
        // Disable seeking & wandering
        fsm.seekSteeringBehaviour.enabled = false;
        fsm.seekSteeringBehaviour.gameObject.SetActive(false);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);

        ((RabbitEnemy)fsm.slimeEnemy).Dodge();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If dodge is complete, cooldown
        if (!((RabbitEnemy)fsm.slimeEnemy).isDodging)
        {
            fsm.ChangeState(fsm.CooldownStateName);
        }
    }
}
