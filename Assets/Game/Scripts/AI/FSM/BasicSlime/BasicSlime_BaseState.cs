using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicSlime_BaseState : FSMBaseState<BasicSlime_FSM>
{
    public override void Init(GameObject _owner, FSM _fsm)
    {
        base.Init(_owner, _fsm);
    }

    // BaseStunDetection can be used in child classes down the inheritance chain
    // Sometimes child classes might want to detect stun effect without calling OnStateUpdate,
    // so its better to have this method separately.
    protected bool BaseStunDetection(AnimatorStateInfo stateInfo)
    {
        // Status effects trigger state changes from any state, hence we put this in the base state
        // Ensure all overrides call base.OnStateUpdate
        if (fsm.slimeEnemy.stunned && stateInfo.shortNameHash != fsm.StunnedStateName)
        {
            // TODO: Need to check if not already in state
            fsm.ChangeState(fsm.StunnedStateName);
            return true;
        }
        return false;
    }

    protected bool ForceBackToBase(AnimatorStateInfo stateInfo)
    {
        if (fsm.wanderSteeringBehaviour.wanderBounds == null) return false;
        if (fsm.wanderSteeringBehaviour.InOuterBounds) return false;

        fsm.ChangeState(fsm.BackToBaseStateName);
        return true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        BaseStunDetection(stateInfo);
        ForceBackToBase(stateInfo);
    }
}
