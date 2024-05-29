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

    // Constructor to initialize default values
    public AttackMove(AnimationClip _clip = null, float _duration = 0.5f, float _animationOffset = 0.0f, Vector2 _direction = default(Vector2))
    {
        this.clip = _clip;
        this.animationOffset = _animationOffset;
        this.duration = _duration;
        this.direction = _direction;
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