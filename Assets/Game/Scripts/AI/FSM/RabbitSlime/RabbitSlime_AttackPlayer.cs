using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_AttackPlayer : BasicSlime_AttackPlayer
{
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // If enemy eye is normal, change it to attack
        if (fsm.slimeEnemy.GetEnemyEye() == EnemyEye.NORMAL)
        {
            fsm.slimeEnemy.SetEye(EnemyEye.ATTACK);
        }

        // Once attack is complete, go to wander state
        if (fsm.slimeAgent.reachedGoal || fsm.weapon.DidAttackLand())
        {
            fsm.ChangeState(fsm.WanderAroundStateName);
        }
    }
}
