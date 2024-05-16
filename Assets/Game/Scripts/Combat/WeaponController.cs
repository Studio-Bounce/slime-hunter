using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    [Tooltip("Assumes the weapons prefab handle is at positioned at origin (0,0,0)")]
    public Transform handPivot;
    public Vector3 handPivotOffset;
    public Vector3 handPivotForward;
    public WeaponSO[] availableWeapons = new WeaponSO[3];
    private int _equippedWeaponIndex;
    private GameObject _currentWeaponPrefab;

    private PlayerInputActions _inputActions;
    private PlayerInputActions.PlayerActions _playerActions;
    private Animator _animator;
    private readonly int attackTriggerHash = Animator.StringToHash("Attack");

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _playerActions = _inputActions.Player;
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        Debug.Assert(handPivot != null, "Requires hand location for weapon");
        if (availableWeapons[0] != null)
        {
            InstantiateWeapon(availableWeapons[0]);
        }

        // Setup input callbacks
        _playerActions.Attack.performed += Attack;
        _playerActions.CycleWeapon.performed += CycleWeapon;
    }

    // WIP: Should instantiate all weapons to begin with and disable as needed
    // BUG: Pivot isn't correct if switching while moving
    private void InstantiateWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;
        _currentWeaponPrefab = Instantiate(weaponSO.weaponModel, handPivot);
        _currentWeaponPrefab.transform.forward = handPivotForward;
        _currentWeaponPrefab.transform.position += handPivotOffset;

        Weapon weaponComponent = _currentWeaponPrefab.AddComponent<Weapon>( );
        weaponComponent?.Setup(weaponSO);
    }

    private void CycleWeapon(InputAction.CallbackContext context)
    {
        // Cycle equipped
        _equippedWeaponIndex = _equippedWeaponIndex == availableWeapons.Length-1 ? 
            0 : _equippedWeaponIndex+1;

        if (_currentWeaponPrefab != null)
        {
            Destroy(_currentWeaponPrefab);
        }

        InstantiateWeapon(availableWeapons[_equippedWeaponIndex]);
    }

    void Attack(InputAction.CallbackContext context)
    {
        Vector2 clickPosition = Mouse.current.position.ReadValue();
        Vector2 currentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = (clickPosition - currentScreenPos).normalized;
        transform.forward = new Vector3(direction.x, 0, direction.y);

        _animator.SetTrigger(attackTriggerHash);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(handPivot.position + handPivotOffset, 0.03f);
            Gizmos.DrawSphere(handPivot.position + handPivotOffset, 0.01f);
            Gizmos.DrawRay(handPivot.position + handPivotOffset, handPivotForward);
        }
    }
#endif
}
