using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackMove
{
    public AnimationClip clip;
    public float duration;
    public float range;
    public float animationOffset;
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public Damage damage;
    public int swingSpeed;
    public int range;
    public LayerMask hitLayers;
    public GameObject weaponModel;
    public AudioClip hitSound;
    public List<AttackMove> attackMoves;
}