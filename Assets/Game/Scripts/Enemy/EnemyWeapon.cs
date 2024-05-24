using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : DamageDealer
{
    [SerializeField] WeaponSO slimeHitWeapon;

    private void Start()
    {
        damage = slimeHitWeapon.damage;
        hitLayers = slimeHitWeapon.hitLayers;
    }

    public void ActivateWeapon()
    {
        active = true;
    }

    public void DeactivateWeapon()
    {
        active = false;
    }
}
