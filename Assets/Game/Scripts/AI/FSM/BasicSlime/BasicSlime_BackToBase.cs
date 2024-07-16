using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackToBase : BasicSlime_BaseState
{
    private float elapsedSeconds = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Enter");
        base.OnStateEnter(animator, stateInfo, layerIndex);
        fsm.LockStateForSeconds(fsm.backToBaseLockedDuration);

        fsm.slimeAgent.reachedGoal = false;
        fsm.seekSteeringBehaviour.enabled = true;
        fsm.seekSteeringBehaviour.gameObject.SetActive(true);
        fsm.seekSteeringBehaviour.target = fsm.wanderSteeringBehaviour.wanderBounds.transform.position;
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);

        fsm.slimeAgent.maxSpeed = fsm.backToBaseSpeed;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedSeconds += Time.deltaTime;
        if (elapsedSeconds > fsm.backToBaseLockedDuration)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }
}
