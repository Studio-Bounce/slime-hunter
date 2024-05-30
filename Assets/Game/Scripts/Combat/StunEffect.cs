using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect/StunEffect")]
public class StunEffect : StatusEffect
{
    protected override void OnStartEffect(DamageTaker taker)
    {
        Debug.Log($"[{taker.gameObject.name}] is stunned");
    }

    protected override void OnUpdateEffect(DamageTaker taker)
    {
        Debug.Log($"[{taker.gameObject.name}] stun tick");
    }

    protected override void OnEndEffect(DamageTaker taker)
    {
        Debug.Log($"[{taker.gameObject.name}] is no longer stunned");
    }
}
