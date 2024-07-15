using System.Collections;
using UnityEngine;

public class BasicSlime_AttackPlayer : BasicSlime_BaseState
{
    readonly int AttackChargeAnimation = Animator.StringToHash("attackChargeUp");
    readonly int AttackReachedAnimation = Animator.StringToHash("attackReached");
    readonly int AttackCompleteAnimation = Animator.StringToHash("attackComplete");

    readonly int ChargeUpState = Animator.StringToHash("HeadButt_ChargeUp");
    readonly int MoveState = Animator.StringToHash("HeadButt_Move");
    readonly int HeadAttackState = Animator.StringToHash("HeadButt_Attack");

    public enum SlimeAttackState
    {
        CHARGE_UP,
        DASH,
        ATTACK,
        NONE
    };

    protected int nextStateName = 0;
    Vector3 target = Vector3.zero;
    bool waitForAnimation = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        nextStateName = fsm.CooldownStateName;
        fsm.currentAttackState = SlimeAttackState.CHARGE_UP;
        // No movement
        fsm.seekSteeringBehaviour.enabled = false;
        fsm.seekSteeringBehaviour.gameObject.SetActive(false);
        fsm.wanderSteeringBehaviour.enabled = false;
        fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
        fsm.slimeAgent.reachedGoal = true;
        fsm.slimeAgent.velocity = Vector3.zero;

        // Change slime material (color)
        if (fsm.slimeOuterMesh.materials.Length > 0)
        {
            Material redGlow = fsm.attackMat;
            // Ensure the material has an emission property
            if (redGlow.HasProperty("_EmissionColor"))
            {
                // Enable the emission keyword
                redGlow.EnableKeyword("_EMISSION");

                // Set the HDR emission color
                Color finalEmissionColor = redGlow.color;
                redGlow.SetColor("_EmissionColor", finalEmissionColor);
            }
            Material[] mats = { redGlow };
            fsm.slimeOuterMesh.materials = mats;
        }
        // Disable shadow in slime outer mesh to show transparent material properly
        fsm.slimeOuterMesh.gameObject.layer = GameConstants.IgnoreLightingLayer;

        fsm.Emit(10); 

        // Change eye
        fsm.slimeEnemy.SetEye(EnemyEye.ATTACK);

        // Attack animation
        fsm.slimeAnimator.SetTrigger(AttackChargeAnimation);
        waitForAnimation = true;

        // Set & look at target
        target = GetSlimeTargetConsideringBoundary();
        fsm.transform.LookAt(target);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        // If enemy eye is normal, change it to attack
        if (fsm.slimeEnemy.GetEnemyEye() == EnemyEye.NORMAL)
        {
            fsm.slimeEnemy.SetEye(EnemyEye.ATTACK);
        }

        // Wait for an attack animation to start (it can take a couple of frames)
        int animationHash = fsm.slimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        while (waitForAnimation && animationHash != ChargeUpState && animationHash != MoveState && animationHash != HeadAttackState)
            return;
        waitForAnimation = false;

        switch (fsm.currentAttackState)
        {
            case SlimeAttackState.CHARGE_UP:
                // Check if charge up has been finished
                if (animationHash != ChargeUpState)
                {
                    // Make the weapon active
                    fsm.weapon.ActivateWeapon();
                    fsm.currentAttackState = SlimeAttackState.DASH;
                }

                // If the slime is already very close to the player, its attack won't get detected (OnTriggerEnter)
                if (Vector3.Distance(fsm.transform.position, target) < 1.0f)
                {
                    // Move the slime back a bit during charge up
                    fsm.characterController.Move(Time.deltaTime * (fsm.transform.position - target).normalized);
                }
                break;

            case SlimeAttackState.DASH:
                // Charge towards target (player)
                Vector3 direction = (target - fsm.transform.position).normalized;
                fsm.characterController.Move(fsm.attackSpeed * Time.deltaTime * direction);

                // If close to goal, do a headbutt
                if (Vector3.Distance(fsm.transform.position, target)  < fsm.attackProximity)
                {
                    fsm.currentAttackState = SlimeAttackState.ATTACK;
                    fsm.slimeAnimator.SetTrigger(AttackReachedAnimation);
                }
                break;

            case SlimeAttackState.ATTACK:
                // Once attack is complete, go to rest state
                bool animationFinished = ((animationHash != ChargeUpState) && (animationHash != MoveState) && (animationHash != HeadAttackState));
                if (animationFinished || fsm.weapon.DidAttackLand())
                {
                    fsm.ChangeState(nextStateName);
                }
                break;
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
        // Revert layer back to enemy
        fsm.slimeOuterMesh.gameObject.layer = GameConstants.EnemyLayer;

        // Make the weapon inactive
        fsm.weapon.DeactivateWeapon();

        // Change eye
        fsm.slimeEnemy.SetEye(EnemyEye.NORMAL);

        // Ensure animation does not get stuck on dash
        fsm.slimeAnimator.SetTrigger(AttackCompleteAnimation);
    }

    Vector3 GetSlimeTargetConsideringBoundary()
    {
        Vector3 lineDirection = fsm.GetPlayerPosition() - fsm.transform.position;
        Ray ray = new(fsm.transform.position, lineDirection);
        int layerMask = (1 << GameConstants.EnemyBoundaryLayer);
        if (Physics.Raycast(ray, out RaycastHit hit, lineDirection.magnitude, layerMask))
        {
            // Little offset, just so that the target is not directly on the wall
            return (hit.point - 0.5f * lineDirection.normalized);
        }
        return fsm.GetPlayerPosition();
    }
}
