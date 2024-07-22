using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberSlime_Explode : BasicSlime_BaseState
{
    BomberSlime_FSM bFSM;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bFSM = (BomberSlime_FSM)fsm;

        // Disable behaviours
        bFSM.seekSteeringBehaviour.enabled = false;
        bFSM.seekSteeringBehaviour.gameObject.SetActive(false);
        bFSM.wanderSteeringBehaviour.enabled = false;
        bFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
        bFSM.slimeAgent.reachedGoal = true;
        bFSM.slimeAgent.velocity = Vector3.zero;

        bFSM.slimeAnimator.SetTrigger(bFSM.PlayerInRangeTrigger);
        bFSM.Explode();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Die
        bFSM.ChangeState(bFSM.DeadStateName);
    }
}
