using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSlime_BaseState : FSMBaseState<BasicSlime_FSM>
{
    public override void Init(GameObject _owner, FSM _fsm)
    {
        base.Init(_owner, _fsm);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        // Status effects trigger state changes from any state, hence we put this in the base state
        // Ensure all overrides call base.OnStateUpdate
        if (fsm.slimeEnemy.stunned && stateInfo.shortNameHash != fsm.StunnedStateName)
        {
            // WIP: Need to check if not already in state
            fsm.ChangeState(fsm.StunnedStateName);
        }
    }
}
