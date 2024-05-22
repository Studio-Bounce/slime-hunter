using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_AttackPlayer : BasicSlime_BaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        fsm.seekSteeringBehaviour.target = fsm.playerTransform.position;
        fsm.slimeAgent.reachedGoal = false;
        fsm.slimeAgent.maxSpeed = fsm.attackSpeed;

        // Enable seeking, disable wandering
        fsm.seekSteeringBehaviour.enabled = true;
        fsm.seekSteeringBehaviour.gameObject.SetActive(true);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float targetDistance = Vector3.Distance(fsm.slimeAgent.transform.position, fsm.seekSteeringBehaviour.target);

        // Once attack is complete, go to rest state
        // HACK: Get rid of the 0.75f. With attack animation, detect attack completion on animation completion
        if (fsm.slimeAgent.reachedGoal || targetDistance < 0.75f)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }
}
