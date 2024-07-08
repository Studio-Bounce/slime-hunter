using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rabbit slime sneaks up on player
// Its target is behind the player
public class RabbitSlime_ChasePlayer : BasicSlime_ChasePlayer
{
    public override void SetChaseTarget()
    {
        // .25f buffer to ensure chase target lies within attack radius
        fsm.seekSteeringBehaviour.target = fsm.GetPlayerPosition() - ((fsm.attackRadius - 0.25f) * fsm.GetPlayerForward());
    }
}
