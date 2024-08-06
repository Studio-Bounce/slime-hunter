using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RabbitSlime_WanderAround : BasicSlime_BaseState
{
    RabbitSlime_FSM rFSM = null;
    bool isMoving = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rFSM = (RabbitSlime_FSM) fsm;

        // Disable all movement
        rFSM.wanderSteeringBehaviour.enabled = false;
        rFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
        rFSM.seekSteeringBehaviour.enabled = false;
        rFSM.seekSteeringBehaviour.gameObject.SetActive(false);

        rFSM.slimeAgent.reachedGoal = true;
        rFSM.slimeAgent.maxSpeed = 0;
        
        isMoving = false;
        rFSM.ResetRestOnNextUpdate = true;

        rFSM.slimeEnemy.Alerted = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (BaseStatusDetection(stateInfo) || ForceBackToBase(stateInfo))
            return;
        
        if (rFSM.IsResting && isMoving)
        {
            // Stop movement
            isMoving = false;
            rFSM.wanderSteeringBehaviour.enabled = false;
            rFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
            rFSM.slimeAgent.reachedGoal = true;
            rFSM.slimeAgent.maxSpeed = 0;
        }
        else if (!rFSM.IsResting && !isMoving)
        {
            // Start movement
            isMoving = true;
            rFSM.wanderSteeringBehaviour.enabled = true;
            rFSM.wanderSteeringBehaviour.gameObject.SetActive(true);
            rFSM.slimeAgent.reachedGoal = false;
            rFSM.slimeAgent.maxSpeed = rFSM.wanderSpeed;
        }

        // If slime reached close to player, switch to chase
        if (Vector3.Distance(rFSM.transform.position, rFSM.GetPlayerPosition()) <= rFSM.seekDistance)
        {
            rFSM.slimeEnemy.Alerted = true;
            rFSM.ChangeState(rFSM.ChasePlayerStateName);
        }
    }
}
