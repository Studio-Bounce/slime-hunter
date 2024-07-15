using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashSlime_FSM : BasicSlime_FSM
{
    [Header("DashSlime Attributes")]
    public float dashDuration;
    public float dashDistance;
    public AnimationCurve dashCurve;
}
