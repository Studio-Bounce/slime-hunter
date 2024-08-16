using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlime_AttackPlayer : ProjectileSlime_BaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // No movement
        projFSM.wanderSteeringBehaviour.enabled = false;
        projFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
        projFSM.slimeAgent.reachedGoal = true;
        projFSM.slimeAgent.velocity = Vector3.zero;
        projFSM.Attack();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // If player came too close, evade
        if (Vector3.Distance(projFSM.transform.position, projFSM.GetPlayerPosition()) <= projFSM.playerProximityDistance)
        {
            projFSM.ChangeState(projFSM.FleeStateName);
        }
        // If player went too far, go back to wandering
        else if (Vector3.Distance(projFSM.transform.position, projFSM.GetPlayerPosition()) > projFSM.seekDistance)
        {
            projFSM.ChangeState(projFSM.WanderAroundStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        projFSM.StopAttacking();
    }
}
