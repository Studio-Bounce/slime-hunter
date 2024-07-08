using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackMove
{
    [Header("Animation Properties")]
    public AnimationClip clip;
    public float attackDelay; // When the attack starts in the animation
    public float duration = 0.5f; // How long the attacks lasts
    public Vector2 direction;

    [Header("Attack Attributes")]
    public float damageMultiplier = 1.0f;
    public float rangeMultiplier = 1.0f;
    public float knockbackMultiplier = 1.0f;

    [Header("Material Properties")]
    public float rotateSpeed;
    [Range(0, 360)] public float angleRange;
    [Range(0, 360)] public float angleStart;
    public float rotationGamma;
    [Range(0, 1)] public float voranoiPeak;
}

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    [Header("Weapon Attributes")]
    public string weaponName;
    public Damage damage;
    public int swingSpeed;
    public float range;
    public LayerMask hitLayers;

    [Header("References")]
    public GameObject weaponModel;
    public AudioClip hitSound;
    public Material material;
    public List<AttackMove> attackMoves;
}