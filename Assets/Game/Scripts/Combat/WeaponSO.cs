using System.Collections.Generic;
using UnityEngine;

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
    public List<AnimationClip> attackMoves;
}