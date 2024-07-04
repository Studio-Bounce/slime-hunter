using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSlime_BaseState : BasicSlime_BaseState
{
    protected ProjectileSlime_FSM projFSM = null;

    public override void Init(GameObject _owner, FSM _fsm)
    {
        base.Init(_owner, _fsm);

        projFSM = (ProjectileSlime_FSM)fsm;
    }
}
