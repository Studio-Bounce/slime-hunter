using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileSlimeBullet : DamageDealer
{
    [SerializeField] WeaponSO bulletWeapon;
    [SerializeField] private ParticleSystem effect;
    [NonSerialized] private bool markDestroy = false;
    [SerializeField] private LayerMask destroyLayers;

    [Header("Slime Projectile")]
    public float speed = 0.0f;
    public Vector3 direction = Vector3.zero;
    public float lifetime = 10.0f;
    float timeLapsed = 0;

    protected override void Start()
    {
        base.Start();
        Active = true;
        timeLapsed = 0;
        damage = bulletWeapon.damage;
        hitLayers = bulletWeapon.hitLayers;

        applyCameraShake = false;
        Destroy(gameObject, lifetime);
    }

    protected override void Update()
    {
        if (markDestroy) return;
        base.Update();

        transform.Translate(speed * Time.deltaTime * direction);

        if (attackDetected || timeLapsed > lifetime)
        {
            markDestroy = true;
            StartCoroutine(PlayImpactEffect());
        }
        timeLapsed += Time.deltaTime;
    }

    private IEnumerator PlayImpactEffect()
    {
        effect.Play();
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        if (bulletWeapon != null)
        {
            damage = bulletWeapon.damage;
            hitLayers = bulletWeapon.hitLayers;
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if ((destroyLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            markDestroy = true;
            StartCoroutine(PlayImpactEffect());
        }
    }
}
