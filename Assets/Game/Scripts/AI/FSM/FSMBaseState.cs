using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base state for all FSM states
/// </summary>
public abstract class FSMBaseState<T> : InternalFSMBaseState where T : FSM
{
    protected GameObject owner { get; private set; }
    protected T fsm { get; private set; }

    public override void Init(GameObject _owner, FSM _fsm)
    {
        owner = _owner;
        fsm = (T)_fsm;
        Debug.Assert(fsm != null, $"{owner.name} requires a {typeof(T)} FSM");
    }
}
