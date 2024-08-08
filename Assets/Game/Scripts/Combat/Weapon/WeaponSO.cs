using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackMove
{
    [Header("Animation Properties")]
    public AnimationClip clip;
    public float animationDelay; // When the attack starts in the animation
    public float animationDuration = 0.5f; // How long the attacks lasts
    public bool flip;
    public float rotation;

    [Header("Attack Attributes")]
    public float attackDuration = 0.3f; // How long to combo into next attack
    public float comboDuration; // How long to combo into next attack
    public float damageMultiplier = 1.0f;
    public float rangeMultiplier = 1.0f;
    public float knockbackMultiplier = 1.0f;

    [Header("Material Properties")]
    public float rotateSpeed;
    [Range(0, 360)] public float angleRange;
    [Range(0, 360)] public float angleStart;
    public float rotationGamma;
    [Range(0, 1)] public float voranoiPeak;

    [Header("Attack Audio")]
    public FMODUnity.EventReference audioHitEvent;
}

[CreateAssetMenu(menuName = "Weapon")]
public class WeaponSO : ItemSO
{
    [Header("Weapon Attributes")]
    public Damage damage;
    public float range;
    public LayerMask hitLayers;

    [Header("Weapon References")]
    public GameObject weaponModel;
    public Material material;
    public List<AttackMove> attackMoves;
}