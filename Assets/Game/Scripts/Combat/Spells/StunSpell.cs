using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.VFX;

[RequireComponent(typeof(SphereCollider), typeof(DamageDealer))]
[System.Serializable]
public class StunSpell : Spell
{
    public ParticleSystem impactEffect;
    public GameObject projectile;
    public EventReference impactSound;

    [Header("Animation")]
    public float animDuration = 1.0f;
    public float animHeight = 2;
    public AnimationCurve animCurve;
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
        impactEffect.transform.localScale = new Vector3(spellSO.areaOfEffect, spellSO.areaOfEffect, spellSO.areaOfEffect);
    }

    public override void Cast(Vector3 target = default)
    {
        StartCoroutine(RunLaunch(transform.position, target));
    }

    IEnumerator RunLaunch(Vector3 start, Vector3 target)
    {
        // Timer and lerp variable initialization
        float timer = 0;
        animDuration = 1.0f;
        Vector2 startVec2 = new Vector2(start.x, start.z);
        Vector2 endVec2 = new Vector2(target.x, target.z);
        float startHeight = start.y;

        // Calculate lerp sin curve
        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float normalTime = timer / animDuration;
            float eased = Easing.EaseOut(normalTime);
            Vector2 lerpedPos = Vector2.LerpUnclamped(startVec2, endVec2, eased);
            eased = animCurve.Evaluate(normalTime);
            float lerpedHeight = startHeight + Mathf.Sin(eased * Mathf.PI) * animHeight;

            transform.position = new Vector3(lerpedPos.x, lerpedHeight, lerpedPos.y);
            yield return null;
        }

        // Activate damage and effects on impact
        RuntimeManager.PlayOneShot(impactSound);
        projectile.gameObject.SetActive(false);
        impactEffect.Play();
        damageDealer.Active = true;
        yield return new WaitForSeconds(0.2f);
        damageDealer.Active = false;
        yield return new WaitForSeconds(impactDuration);
        Destroy(gameObject);
    }
}
