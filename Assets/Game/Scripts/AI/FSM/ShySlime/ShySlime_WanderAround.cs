using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShySlime_WanderAround : BasicSlime_WanderAround
{
    private ShySlime_FSM sFSM;
    private FleeSteeringBehaviour fleeSteeringBehaviour;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sFSM = fsm as ShySlime_FSM;
        fsm.wanderSteeringBehaviour.enabled = true;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(true);
        sFSM.fleeSteeringBehaviour.enabled = true;
        sFSM.fleeSteeringBehaviour.gameObject.SetActive(true);
        fsm.slimeAgent.reachedGoal = false;
        fsm.slimeAgent.maxSpeed = fsm.wanderSpeed;
        fsm.slimeEnemy.SetEye(EnemyEye.NORMAL);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // If slime reached close to player, switch to flee
        if (Vector3.Distance(fsm.transform.position, fsm.GetPlayerPosition()) <= sFSM.playerProximityDistance)
        {
            fsm.ChangeState(sFSM.FleePlayerStateName);
        }
    }
}
