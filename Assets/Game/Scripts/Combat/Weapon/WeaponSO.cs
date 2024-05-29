using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AttackMove
{
    public AnimationClip clip;
    public float animationOffset; // When the attack starts in the animation
    public float duration; // How long the attacks lasts
    public Vector2 direction;
    public float rangeMultiplier;

    // Constructor to initialize default values
    public AttackMove(AnimationClip _clip = null, float _duration = 0.5f, float _offset = 0.0f, Vector2 _dir = default(Vector2), float _range = 1.0f)
    {
        this.clip = _clip;
        this.animationOffset = _offset;
        this.duration = _duration;
        this.direction = _dir;
        this.rangeMultiplier = _range;
    }
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