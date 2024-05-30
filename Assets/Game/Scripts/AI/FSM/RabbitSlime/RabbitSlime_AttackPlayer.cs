using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_AttackPlayer : BasicSlime_AttackPlayer
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        nextStateName = fsm.WanderAroundStateName;
    }
}
