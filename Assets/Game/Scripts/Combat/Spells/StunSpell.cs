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

    [Header("Animation")]
    public float projectileDuration = 1.0f;
    public AnimationCurve projectileCurve;
    public float impactDuration = 5.0f;

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
        projectileDuration = 1.0f;
        Vector2 startVec2 = new Vector2(start.x, start.z);
        Vector2 endVec2 = new Vector2(target.x, target.z);

        // Calculate lerp sin curve
        while (timer < projectileDuration)
        {
            timer += Time.deltaTime;
            float eased = projectileCurve.Evaluate(timer);
            Vector2 lerpedPos = Vector2.LerpUnclamped(startVec2, endVec2, eased);
            transform.position = new Vector3(lerpedPos.x, transform.position.y, lerpedPos.y);
            yield return null;
        }

        // Activate damage and effects on impact
        projectile.gameObject.SetActive(false);
        impactEffect.Play();
        damageDealer.Active = true;
        yield return new WaitForSeconds(0.2f);
        damageDealer.Active = false;
        yield return new WaitForSeconds(impactDuration);
        Destroy(gameObject);
    }


}
