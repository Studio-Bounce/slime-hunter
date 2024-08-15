using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberSlime_WanderAround : BasicSlime_WanderAround
{
    BomberSlime_FSM bFSM;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        bFSM = (fsm as BomberSlime_FSM);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stun & force to base detection
        if (BaseStatusDetection(stateInfo) || ForceBackToBase(stateInfo))
        {
            bFSM.slimeAnimator.SetTrigger(bFSM.PlayerLostTrigger);
            return;
        }

        // If slime reached close to player, switch to chase
        if (Vector3.Distance(bFSM.transform.position, bFSM.GetPlayerPosition()) <= bFSM.seekDistance)
        {
            bFSM.slimeEnemy.Alerted = true;
            bFSM.ChangeState(bFSM.ChasePlayerStateName);
        }
    }
}
