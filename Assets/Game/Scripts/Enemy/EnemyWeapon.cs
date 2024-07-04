using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : DamageDealer
{
    [SerializeField] WeaponSO slimeHitWeapon;

    protected override void Start()
    {
        base.Start();
        damage = slimeHitWeapon.damage;
        hitLayers = slimeHitWeapon.hitLayers;
    }

    public bool DidAttackLand()
    {
        return attackDetected;
    }

    public void ActivateWeapon()
    {
        active = true;
    }

    public void DeactivateWeapon()
    {
        active = false;
    }

    private void OnValidate()
    {
        if (slimeHitWeapon != null)
        {
            damage = slimeHitWeapon.damage;
            hitLayers = slimeHitWeapon.hitLayers;
        }
    }
}
