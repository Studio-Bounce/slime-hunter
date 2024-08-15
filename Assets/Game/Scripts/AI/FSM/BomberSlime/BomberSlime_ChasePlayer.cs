using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberSlime_ChasePlayer : BasicSlime_ChasePlayer
{
    BomberSlime_FSM bFSM;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        bFSM = (BomberSlime_FSM)fsm;
        // Update position as per correct logic
        bFSM.seekSteeringBehaviour.target = bFSM.GetPlayerPosition();
        // Alert the slime
        if (!bFSM.alertTriggerOverride)
        {
            bFSM.slimeAnimator.SetTrigger(bFSM.AlertTrigger);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stun & force to base detection
        if (BaseStatusDetection(stateInfo) || ForceBackToBase(stateInfo))
        {
            bFSM.slimeAnimator.SetTrigger(bFSM.PlayerLostTrigger);
            return;
        }

        // Player can move so keep adjusting target
        SetChaseTarget();

        float playerDistance = Vector3.Distance(bFSM.slimeAgent.transform.position, bFSM.GetPlayerPosition());
        // If player evaded, switch back to wander
        if (playerDistance > bFSM.seekDistance)
        {
            bFSM.slimeAnimator.SetTrigger(bFSM.PlayerLostTrigger);
            bFSM.ChangeState(bFSM.WanderAroundStateName);
        }
        // If player is within attack radius, attack him
        else if (playerDistance < bFSM.attackRadius)
        {
            bFSM.ChangeState(bFSM.AttackPlayerStateName);
        }
    }
}
