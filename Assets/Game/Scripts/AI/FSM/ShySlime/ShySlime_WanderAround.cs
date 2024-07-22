using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShySlime_WanderAround : BasicSlime_WanderAround
{
    private ShySlime_FSM sFSM;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sFSM = fsm as ShySlime_FSM;
        fsm.seekSteeringBehaviour.enabled = false;
        fsm.seekSteeringBehaviour.gameObject.SetActive(false);
        fsm.wanderSteeringBehaviour.enabled = true;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(true);
        sFSM.fleeSteeringBehaviour.enabled = false;
        sFSM.fleeSteeringBehaviour.gameObject.SetActive(false);
        fsm.slimeAgent.reachedGoal = false;
        fsm.slimeAgent.maxSpeed = fsm.wanderSpeed;
        fsm.slimeEnemy.SetEye(EnemyEye.NORMAL);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BaseStatusDetection(stateInfo);
        ForceBackToBase(stateInfo);
        // If slime reached close to player, switch to flee
        if (Vector3.Distance(fsm.transform.position, fsm.GetPlayerPosition()) <= sFSM.playerProximityDistance)
        {
            fsm.ChangeState(sFSM.FleePlayerStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
    }
}
