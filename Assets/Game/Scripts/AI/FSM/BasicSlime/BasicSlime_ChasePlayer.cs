using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_ChasePlayer : BasicSlime_BaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fsm.seekSteeringBehaviour.target = fsm.playerTransform.position;
        fsm.slimeAgent.reachedGoal = false;
        fsm.slimeAgent.maxSpeed = fsm.chaseSpeed;

        // Enable seeking, disable wandering
        fsm.seekSteeringBehaviour.enabled = true;
        fsm.seekSteeringBehaviour.gameObject.SetActive(true);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Player can move so keep adjusting target
        fsm.seekSteeringBehaviour.target = fsm.playerTransform.position;

        float playerDistance = Vector3.Distance(fsm.slimeAgent.transform.position, fsm.playerTransform.position);
        // If player evaded, switch back to wander
        if (playerDistance > fsm.seekDistance)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
        // If player is within attack radius, attack him
        else if (playerDistance < fsm.attackRadius)
        {
            fsm.ChangeState(fsm.AttackPlayerStateName);
        }
    }
}
