using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSlime_FSM : BasicSlime_FSM
{
    [Header("DashSlime Attributes")]
    public float dashDuration;
    public float dashDistance;
    public float attackDelay;
    public AnimationCurve dashCurve;

    // Used in tutorial. DO NOT USE ANYWHERE ELSE
    public void SetPlayerTransform(Transform _playerT)
    {
        playerTransform = _playerT;
    }
}
