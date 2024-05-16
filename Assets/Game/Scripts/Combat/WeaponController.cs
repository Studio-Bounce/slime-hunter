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

        // Spawn Initial Weapon
        InitializeHandPivot();
        InstantiateWeapon(availableWeapons[0]);

        // Setup input callbacks
        _playerActions.Attack.performed += Attack;
        _playerActions.CycleWeapon.performed += CycleWeapon;
    }

    private void InitializeHandPivot()
    {
        // Create new pivot transform as a child so we don't influence the original hand rotation
        GameObject newPivot = new GameObject("HandPivot");
        newPivot.transform.SetParent(handPivot.transform);
        newPivot.transform.localPosition = Vector3.zero;
        handPivot = newPivot.transform;
        handPivot.forward = handPivotForward;
    }

    // WIP: Should instantiate all weapons to begin with and disable as needed
    // BUG: Pivot isn't correct if switching while moving
    private void InstantiateWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;
        _currentWeaponPrefab = Instantiate(weaponSO.weaponModel, handPivot);
        _currentWeaponPrefab.transform.forward = handPivot.forward;
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
        // Get vector from player to mouse click
        Vector2 clickPosition = Mouse.current.position.ReadValue();
        Vector2 currentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 clickDirection = (clickPosition - currentScreenPos).normalized;

        // Align to camera forward
        Vector3 cameraToPlayer = (transform.position - Camera.main.transform.position);
        Vector2 forwardDirection = new Vector2(cameraToPlayer.x, cameraToPlayer.z);
        forwardDirection.Normalize();
        Vector2 rightDirection = new Vector2(forwardDirection.y, -forwardDirection.x);

        Vector2 finalDirection = (forwardDirection * clickDirection.y + rightDirection * clickDirection.x).normalized;

        transform.forward = new Vector3(finalDirection.x, 0, finalDirection.y);

        _animator.SetTrigger(attackTriggerHash);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 pivotWithOffset = handPivot.position + handPivotOffset;
        Gizmos.DrawWireSphere(pivotWithOffset, 0.03f);
        Gizmos.DrawSphere(pivotWithOffset, 0.01f);

        if (Application.isPlaying)
        {
            Gizmos.DrawRay(pivotWithOffset, handPivot.forward);
        } else
        {
            Gizmos.DrawRay(pivotWithOffset, handPivotForward);
        }
    }
#endif
}
