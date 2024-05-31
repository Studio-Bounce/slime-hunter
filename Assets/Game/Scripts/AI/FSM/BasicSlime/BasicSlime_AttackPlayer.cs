using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WeaponController;

public class BasicSlime_AttackPlayer : BasicSlime_BaseState
{
    readonly int ToIdleAnimation = Animator.StringToHash("toIdle");
    readonly int AttackChargeAnimation = Animator.StringToHash("attackChargeUp");
    readonly int AttackReachedAnimation = Animator.StringToHash("attackReached");

    readonly int ChargeUpState = Animator.StringToHash("HeadButt_ChargeUp");
    readonly int MoveState = Animator.StringToHash("HeadButt_Move");
    readonly int HeadAttackState = Animator.StringToHash("HeadButt_Attack");

    enum AttackState
    {
        CHARGE_UP,
        DASH,
        ATTACK
    };
    AttackState attackState;

    protected int nextStateName = 0;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        nextStateName = fsm.CooldownStateName;
        attackState = AttackState.CHARGE_UP;
        UpdateSteeringBehaviours();

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
        fsm.StartCoroutine(IncreaseEmissionIntensity(10));


        // Make the weapon active
        fsm.weapon.ActivateWeapon();

        // Change eye
        fsm.slimeEnemy.SetEye(EnemyEye.ATTACK);

        // Attack animation
        fsm.slimeAnimator.SetTrigger(AttackChargeAnimation);
    }

    IEnumerator IncreaseEmissionIntensity(int steps)
    {
        float intensity = 0.0f;
        float intensityDelta = fsm.attackGlowIntensity / steps;
        float deltaTime = 1.0f / steps;
        while (steps > 0)
        {
            --steps;
            intensity += intensityDelta;

            Color finalEmissionColor = fsm.attackMat.color * intensity;
            fsm.slimeOuterMesh.materials[0].SetColor("_EmissionColor", finalEmissionColor);

            yield return new WaitForSeconds(deltaTime);
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If enemy eye is normal, change it to attack
        if (fsm.slimeEnemy.GetEnemyEye() == EnemyEye.NORMAL)
        {
            fsm.slimeEnemy.SetEye(EnemyEye.ATTACK);
        }

        switch (attackState)
        {
            case AttackState.CHARGE_UP:
                // Check if charge up has been finished
                if (fsm.slimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != ChargeUpState)
                {
                    attackState = AttackState.DASH;
                    UpdateSteeringBehaviours();
                }
                break;

            case AttackState.DASH:
                // If close to goal, do a headbutt
                if (Vector3.Distance(fsm.slimeAgent.transform.position,
                                     fsm.seekSteeringBehaviour.target)  < fsm.attackProximity)
                {
                    attackState = AttackState.ATTACK;
                    fsm.slimeAnimator.SetTrigger(AttackReachedAnimation);
                }
                break;

            case AttackState.ATTACK:
                // Once attack is complete, go to rest state
                int animationHash = fsm.slimeAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash;
                bool animationFinished = ((animationHash != ChargeUpState) && (animationHash != MoveState) && (animationHash != HeadAttackState));
                if (animationFinished && (fsm.slimeAgent.reachedGoal || fsm.weapon.DidAttackLand()))
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

        // Make the weapon inactive
        fsm.weapon.DeactivateWeapon();

        // Change eye
        fsm.slimeEnemy.SetEye(EnemyEye.NORMAL);

        // Ensure the attack animation sequence has been completed
        fsm.slimeAnimator.SetTrigger(ToIdleAnimation);
    }

    void UpdateSteeringBehaviours()
    {
        if (attackState == AttackState.CHARGE_UP)
        {
            // No movement
            fsm.seekSteeringBehaviour.enabled = false;
            fsm.seekSteeringBehaviour.gameObject.SetActive(false);
            fsm.wanderSteeringBehaviour.enabled = false;
            fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
            fsm.slimeAgent.reachedGoal = true;
            fsm.slimeAgent.velocity = Vector3.zero;
        }
        else if (attackState == AttackState.DASH)
        {
            // Steer towards player
            fsm.slimeAgent.reachedGoal = false;
            fsm.slimeAgent.maxSpeed = fsm.attackSpeed;
            fsm.seekSteeringBehaviour.enabled = true;
            fsm.seekSteeringBehaviour.gameObject.SetActive(true);
            fsm.seekSteeringBehaviour.target = fsm.playerTransform.position;
            fsm.wanderSteeringBehaviour.enabled = false;
            fsm.wanderSteeringBehaviour.gameObject.SetActive(false);
        }
    }
}
