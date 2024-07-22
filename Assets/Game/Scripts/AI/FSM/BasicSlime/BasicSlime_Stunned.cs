using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BasicSlime_Stunned : BasicSlime_BaseState
{
    readonly int StunnedBoolHash = Animator.StringToHash("isStunned");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Check if there's any remaining stun effects, else leave
        foreach (StatusEffect effect in fsm.slimeEnemy.activeEffects)
        {
            if (effect is StunEffect)
            {
                return;
            }
        }
        fsm.ChangeState(fsm.WanderAroundStateName);
    }
}