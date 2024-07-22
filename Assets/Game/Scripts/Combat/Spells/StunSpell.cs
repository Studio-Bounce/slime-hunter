using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider), typeof(DamageDealer))]
[System.Serializable]
public class StunSpell : Spell
{
    public ParticleSystem impactEffect;
    public GameObject projectile;

    public AnimationCurve projectileCurve;

    private DamageDealer damageDealer;
    private SphereCollider damageCollider;

    private void Awake()
    {
        damageDealer = GetComponent<DamageDealer>();
        damageDealer.Active = false;
        damageCollider = GetComponent<SphereCollider>();
        damageCollider.isTrigger = true;
    }

    public override void Initialize(SpellSO spellSO)
    {
        damageDealer.damage = spellSO.damage;
        damageCollider.radius = spellSO.areaOfEffect;
    }

    public override void Cast(Vector3 target = default)
    {
        StartCoroutine(RunLaunch(transform.position, target));
    }

    IEnumerator RunLaunch(Vector3 start, Vector3 target)
    {
        // Timer and lerp variable initialization
        float timer = 0;
        float duration = 1.0f;
        float maxHeight = 4;
        Vector2 startVec2 = new Vector2(start.x, start.z);
        Vector2 endVec2 = new Vector2(target.x, target.z);

        // Calculate lerp sin curve
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float eased = projectileCurve.Evaluate(timer);
            Vector2 lerpedPos = Vector2.Lerp(startVec2, endVec2, eased);
            transform.position = new Vector3(lerpedPos.x, transform.position.y, lerpedPos.y);
            yield return null;
        }

        // Activate damage and effects on impact
        projectile.gameObject.SetActive(false);
        impactEffect.Play();
        damageDealer.Active = true;
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }


}
