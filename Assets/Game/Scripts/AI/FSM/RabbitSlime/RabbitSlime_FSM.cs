using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSlime_FSM : BasicSlime_FSM
{
    public readonly int DodgeStateName = Animator.StringToHash("Dodge");
    
    protected override void Start()
    {
        base.Start();
        // Rabbit slime is invincible at the beginning
        slimeEnemy.isInvincible = true;
    }
}
