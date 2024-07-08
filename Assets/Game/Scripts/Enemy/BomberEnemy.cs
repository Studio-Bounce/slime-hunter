using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberEnemy : Enemy
{
    BomberSlime_FSM slimeBombFSM;

    protected override void Start()
    {
        base.Start();

        slimeBombFSM = GetComponent<BomberSlime_FSM>();
    }

    public override void TakeDamage(Damage damage)
    {
        // Explode
        slimeBombFSM.GotHit();
    }
}
