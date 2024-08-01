using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rabbit slime sneaks up on player
// Its target is behind the player
public class RabbitSlime_ChasePlayer : BasicSlime_ChasePlayer
{
    bool reachedBehindPlayer = false;
    Vector3 chaseTarget = Vector3.zero;

    public override void SetChaseTarget()
    {
        if (reachedBehindPlayer)
        {
            chaseTarget = fsm.GetPlayerPosition();
        }
        else
        {
            // .25f buffer to ensure chase target lies within attack radius
            chaseTarget = fsm.GetPlayerPosition() - ((fsm.attackRadius - 0.25f) * fsm.GetPlayerForward());
        }
        fsm.seekSteeringBehaviour.target = chaseTarget;
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        reachedBehindPlayer = false;

        // Move fast
        ((RabbitSlime_FSM)fsm).SetChaseAnimationSpeed();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (BaseStatusDetection(stateInfo) || ForceBackToBase(stateInfo))
            return;

        // Player can move so keep adjusting target
        SetChaseTarget();

        float playerDistance = Vector3.Distance(fsm.slimeAgent.transform.position, chaseTarget);
        float buffer = (reachedBehindPlayer) ? 1.0f : 0.2f;

        // If player evaded, switch back to wander
        if (playerDistance > fsm.seekDistance)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
        // If player is within attack radius, attack him
        else if (playerDistance < buffer)
        {
            if (!reachedBehindPlayer)
            {
                reachedBehindPlayer = true;
                SetChaseTarget();
            }
            else
            {
                fsm.ChangeState(fsm.AttackPlayerStateName);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        ((RabbitSlime_FSM)fsm).ResetMoveAnimationSpeed();
    }
}
