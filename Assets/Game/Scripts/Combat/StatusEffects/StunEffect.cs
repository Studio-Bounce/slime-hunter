using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect/StunEffect")]
public class StunEffect : StatusEffect
{
    protected override void OnStartEffect(DynamicDamageTaker taker)
    {
        taker.stunned = true;
    }

    protected override void OnUpdateEffect(DynamicDamageTaker taker)
    {
    }

    protected override void OnEndEffect(DynamicDamageTaker taker)
    {
        taker.stunned = false;
    }
}
