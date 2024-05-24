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

        // Change slime material (color)
        if (fsm.slimeOuterMesh.materials.Length > 0)
        {
            Material[] mats = { fsm.attackMat };
            fsm.slimeOuterMesh.materials = mats;
        }

        // Make the weapon active
        fsm.weapon.ActivateWeapon();

        // Change eye
        fsm.slimeEnemy.SetEye(EnemyEye.ATTACK);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float targetDistance = Vector3.Distance(fsm.slimeAgent.transform.position, fsm.seekSteeringBehaviour.target);

        // Once attack is complete, go to rest state
        if (fsm.slimeAgent.reachedGoal || fsm.weapon.DidAttackLand())
        {
            fsm.ChangeState(fsm.CooldownStateName);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Revert slime material (color)
        if (fsm.slimeOuterMesh.materials.Length > 0)
        {
            Material[] mats = { fsm.defaultMat };
            fsm.slimeOuterMesh.materials = mats;
        }

        // Make the weapon inactive
        fsm.weapon.DeactivateWeapon();

        // Change eye
        fsm.slimeEnemy.SetEye(EnemyEye.NORMAL);
    }
}
