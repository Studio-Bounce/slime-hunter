using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShySlime_Flee : BasicSlime_BaseState
{
    ShySlime_FSM sFSM;

    public virtual void SetFleeTarget()
    {
        sFSM.fleeSteeringBehaviour.target = sFSM.GetPlayerPosition();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sFSM = fsm as ShySlime_FSM;
        // Enable flee, disable wandering
        sFSM.fleeSteeringBehaviour.enabled = true;
        sFSM.fleeSteeringBehaviour.gameObject.SetActive(true);
        sFSM.wanderSteeringBehaviour.enabled = false;
        sFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
        SetFleeTarget();

        sFSM.slimeAgent.reachedGoal = false;
        sFSM.slimeAgent.maxSpeed = sFSM.fleeSpeed;
        sFSM.slimeEnemy.SetEye(EnemyEye.SCARED);

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Player can move so keep adjusting target
        SetFleeTarget();

        float playerDistance = Vector3.Distance(sFSM.transform.position, sFSM.GetPlayerPosition());
        // If evade was successful, switch back to wander
        if (playerDistance > sFSM.playerProximityDistance)
        {
            sFSM.ChangeState(sFSM.WanderAroundStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        sFSM.fleeSteeringBehaviour.enabled = false;
        sFSM.fleeSteeringBehaviour.gameObject.SetActive(false);
    }
}
