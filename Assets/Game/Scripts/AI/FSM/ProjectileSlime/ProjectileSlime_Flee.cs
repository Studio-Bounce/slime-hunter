using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlime_Flee : ProjectileSlime_BaseState
{
    public virtual void SetFleeTarget()
    {
        projFSM.fleeSteeringBehaviour.target = projFSM.GetPlayerPosition();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Enable flee, disable wandering
        projFSM.fleeSteeringBehaviour.enabled = true;
        projFSM.fleeSteeringBehaviour.gameObject.SetActive(true);
        projFSM.wanderSteeringBehaviour.enabled = false;
        projFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
        SetFleeTarget();

        projFSM.slimeAgent.reachedGoal = false;
        projFSM.slimeAgent.maxSpeed = projFSM.fleeSpeed;

        projFSM.slimeEnemy.SetEye(EnemyEye.NORMAL);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Player can move so keep adjusting target
        SetFleeTarget();

        float playerDistance = Vector3.Distance(projFSM.transform.position, projFSM.GetPlayerPosition());
        // If evade was successful, switch back to wander
        if (playerDistance > projFSM.playerProximityDistance)
        {
            projFSM.ChangeState(projFSM.WanderAroundStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        projFSM.fleeSteeringBehaviour.enabled = false;
        projFSM.fleeSteeringBehaviour.gameObject.SetActive(false);
    }
}
