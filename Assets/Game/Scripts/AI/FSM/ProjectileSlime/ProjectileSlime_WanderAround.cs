using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlime_WanderAround : ProjectileSlime_BaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Enable wandering, disable fleeing
        projFSM.wanderSteeringBehaviour.enabled = true;
        projFSM.wanderSteeringBehaviour.gameObject.SetActive(true);
        projFSM.fleeSteeringBehaviour.enabled = false;
        projFSM.fleeSteeringBehaviour.gameObject.SetActive(false);

        projFSM.slimeAgent.reachedGoal = false;
        projFSM.slimeAgent.maxSpeed = projFSM.wanderSpeed;

        projFSM.slimeEnemy.SetEye(EnemyEye.NORMAL);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BaseStatusDetection(stateInfo);

        // If slime sees the player, start attacking
        if (Vector3.Distance(projFSM.slimeAgent.transform.position, projFSM.GetPlayerPosition()) <= projFSM.seekDistance)
        {
            projFSM.ChangeState(projFSM.AttackPlayerStateName);
        }
    }
}
