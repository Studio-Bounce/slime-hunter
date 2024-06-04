using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_Cooldown : BasicSlime_BaseState
{
    bool isResting = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop the agent
        fsm.slimeAgent.reachedGoal = true;
        // Disable seeking & wandering
        fsm.seekSteeringBehaviour.enabled = false;
        fsm.seekSteeringBehaviour.gameObject.SetActive(false);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);

        // Leverage the fsm script to start a coroutine as fsm state is not monobehaviour
        fsm.StartCoroutine(Rest(fsm.cooldownTime));
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isResting)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }

    IEnumerator Rest(float waitTime)
    {
        isResting = true;
        yield return new WaitForSeconds(waitTime);
        isResting = false;
    }
}
