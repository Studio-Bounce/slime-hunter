using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InternalFSMBaseState : StateMachineBehaviour
{
    abstract public void Init(GameObject _owner, FSM _fsm);
}
