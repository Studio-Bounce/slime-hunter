using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatusEffect/PoisonEffect")]
public class PoisonEffect : StatusEffect
{
    public Damage damage;

    protected override void OnStartEffect(DynamicDamageTaker taker)
    {

    }

    protected override void OnUpdateEffect(DynamicDamageTaker taker)
    {
        damage.direction = (new Vector3(Random.value, 0, Random.value)).normalized;
        taker.TakeDamage(damage, true);
    }

    protected override void OnEndEffect(DynamicDamageTaker taker)
    {

    }
}
