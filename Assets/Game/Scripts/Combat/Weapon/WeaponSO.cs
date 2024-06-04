using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackMove
{
    public AnimationClip clip;
    public float animationOffset; // When the attack starts in the animation
    public float duration = 0.5f; // How long the attacks lasts
    public Vector2 direction;
    public float damageMultiplier = 1.0f;
    public float rangeMultiplier = 1.0f;
    public float knockbackMultiplier = 1.0f;
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public Damage damage;
    public int swingSpeed;
    public float range;
    public LayerMask hitLayers;
    public GameObject weaponModel;
    public AudioClip hitSound;
    public List<AttackMove> attackMoves;
}