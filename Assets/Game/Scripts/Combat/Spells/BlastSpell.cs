using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider), typeof(DamageDealer))]
[System.Serializable]
public class BlastSpell : Spell
{
    public ParticleSystem effect;
    private DamageDealer damageDealer;
    private SphereCollider damageCollider;

    [Header("Animation")]
    public float impactDuration = 5.0f;

    private void Awake()
    {
        damageDealer = GetComponent<DamageDealer>();
        damageDealer.Active = false;
        damageCollider = GetComponent<SphereCollider>();
        damageCollider.isTrigger = true;
    }

    public override void Cast(Vector3 target = default)
    {
        StartCoroutine(RunCast());
    }

    IEnumerator RunCast()
    {
        effect.Play();
        damageDealer.Active = true;
        yield return new WaitForSeconds(0.2f);
        damageDealer.Active = false;
        yield return new WaitForSeconds(impactDuration);
    }

    public override void Initialize(SpellSO spellSO)
    {
        damageDealer.damage = spellSO.damage;
        damageCollider.radius = spellSO.areaOfEffect;
        effect.transform.localScale = new Vector3(spellSO.areaOfEffect, spellSO.areaOfEffect, spellSO.areaOfEffect);
    }
}
