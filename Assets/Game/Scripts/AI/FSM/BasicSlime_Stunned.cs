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
        Debug.Log($"{this.name}:{this.GetInstanceID()} is stunned");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!fsm.slimeEnemy.stunned)
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }
}