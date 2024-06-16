using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect/StunEffect")]
public class StunEffect : StatusEffect
{
    protected override void OnStartEffect(DamageTaker taker)
    {
        taker.stunned = true;
    }

    protected override void OnUpdateEffect(DamageTaker taker)
    {
    }

    protected override void OnEndEffect(DamageTaker taker)
    {
        taker.stunned = false;
    }
}
