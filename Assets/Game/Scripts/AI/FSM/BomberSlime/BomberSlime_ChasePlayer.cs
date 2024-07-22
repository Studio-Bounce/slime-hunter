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
        // Alert the slime
        bFSM.slimeAnimator.SetTrigger(bFSM.AlertTrigger);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stun & force to base detection
        if (BaseStunDetection(stateInfo) || ForceBackToBase(stateInfo))
        {
            bFSM.slimeAnimator.SetTrigger(bFSM.PlayerLostTrigger);
            return;
        }

        // Player can move so keep adjusting target
        SetChaseTarget();

        float playerDistance = Vector3.Distance(fsm.slimeAgent.transform.position, fsm.GetPlayerPosition());
        // If player evaded, switch back to wander
        if (playerDistance > fsm.seekDistance)
        {
            bFSM.slimeAnimator.SetTrigger(bFSM.PlayerLostTrigger);
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
        // If player is within attack radius, attack him
        else if (playerDistance < fsm.attackRadius)
        {
            fsm.ChangeState(fsm.AttackPlayerStateName);
        }
    }
}
