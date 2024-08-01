using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_AttackPlayer : BasicSlime_BaseState
{
    bool waitForAnimation = false;

    readonly int AttackAnimation = Animator.StringToHash("attack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // No movement
        fsm.seekSteeringBehaviour.enabled = false;
        fsm.seekSteeringBehaviour.gameObject.SetActive(false);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
        fsm.slimeAgent.reachedGoal = true;
        fsm.slimeAgent.velocity = Vector3.zero;

        // Look at target
        fsm.transform.LookAt(fsm.GetPlayerPosition());

        fsm.weapon.ActivateWeapon();

        // Attack animation
        fsm.slimeAnimator.SetTrigger(AttackAnimation);
        waitForAnimation = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Wait for an attack animation to start (it can take a couple of frames)
        int animationHash = fsm.slimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        while (waitForAnimation && (animationHash == Animator.StringToHash("Idle") || animationHash == Animator.StringToHash("Move")))
            return;
        waitForAnimation = false;

        fsm.characterController.Move(Time.deltaTime * (fsm.GetPlayerPosition() - fsm.transform.position));

        // If attack over, back to wander
        if (animationHash == Animator.StringToHash("Idle"))
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }
}
