using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Weapon : MonoBehaviour
{
    Transform handPivot;
    WeaponSO weaponSO;

    void Start()
    {
        Debug.Assert(handPivot != null);
        Debug.Assert(weaponSO != null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
