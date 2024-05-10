using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public int damage;
    public GameObject weaponModel;
    public AudioClip hitSound;
}