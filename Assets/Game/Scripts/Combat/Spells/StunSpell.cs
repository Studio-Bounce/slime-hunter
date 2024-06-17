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
    public VisualEffect impactEffect;
    public GameObject projectile;
    private DamageDealer damageDealer;
    private SphereCollider damageCollider;

    private void Start()
    {
        damageDealer = GetComponent<DamageDealer>();
        damageDealer.Active = false;
        damageCollider = GetComponent<SphereCollider>();
        damageCollider.isTrigger = true;
        damageCollider.radius = 3; // WIP: Hardcoded
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
        float startHeight = start.y;
        float maxHeight = 4;
        Vector2 startVec2 = new Vector2(start.x, start.z);
        Vector2 endVec2 = new Vector2(target.x, target.z);

        // Calculate lerp sin curve
        while (timer < duration)
        {
            timer += Time.deltaTime;
            Vector2 lerpedPos = Vector2.Lerp(startVec2, endVec2, timer);
            float lerpedHeight = startHeight + Mathf.Sin(timer*Mathf.PI)*maxHeight;
            transform.position = new Vector3(lerpedPos.x, lerpedHeight, lerpedPos.y);
            yield return null;
        }

        // Activate damage and effects on impact
        projectile.gameObject.SetActive(false);
        impactEffect.Play();
        damageDealer.Active = true;
        yield return new WaitForSeconds(0.2f); // To account for any spawn delay affecting HasAnySystemAwake
        while (impactEffect.HasAnySystemAwake())
        {
            yield return null;
        }
        Destroy(gameObject);
    }


}
