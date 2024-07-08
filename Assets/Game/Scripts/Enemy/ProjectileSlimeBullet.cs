using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileSlimeBullet : DamageDealer
{
    [SerializeField] WeaponSO bulletWeapon;

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
        base.Update();

        transform.Translate(speed * Time.deltaTime * direction);

        if (attackDetected || timeLapsed > lifetime)
        {
            Destroy(gameObject);
        }
        timeLapsed += Time.deltaTime;
    }

    private void OnValidate()
    {
        if (bulletWeapon != null)
        {
            damage = bulletWeapon.damage;
            hitLayers = bulletWeapon.hitLayers;
        }
    }
}
