using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public int damage;
    public int swingSpeed;
    public int range;
    public int knockback;
    public LayerMask hitLayers;
    public GameObject weaponModel;
    public AudioClip hitSound;
}