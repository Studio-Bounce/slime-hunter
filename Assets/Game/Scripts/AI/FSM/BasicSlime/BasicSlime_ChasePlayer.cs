using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_ChasePlayer : BasicSlime_BaseState
{
    [SerializeField] float speed = 10.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fsm.seekSteeringBehaviour.target = fsm.playerTransform.position;
        fsm.slimeAgent.reachedGoal = false;
        fsm.slimeAgent.maxSpeed = speed;

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

        // If player evaded, switch back to wander
        if (Vector3.Distance(fsm.slimeAgent.transform.position, fsm.playerTransform.position) > fsm.seekDistance)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }
}
