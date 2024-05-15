using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : DamageDealer
{
    public void Setup(WeaponSO weaponSO)
    {
        damage = weaponSO.damage;
        hitLayers = weaponSO.hitLayers;
    }
}
