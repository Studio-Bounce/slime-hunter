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
    public WeaponSO weaponSO;

    private PlayerInputActions _inputActions;
    private PlayerInputActions.PlayerActions _playerActions;
    private Animator _animator;

    private int attackTriggerHash;

    private GameObject currentWeapon;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _playerActions = _inputActions.Player;
        _animator = GetComponent<Animator>();
        attackTriggerHash = Animator.StringToHash("Attack");
    }

    void Start()
    {
        Debug.Assert(handPivot != null, "Requires hand location for weapon");
        Debug.Assert(weaponSO != null, "Requires weapon scriptable object");
        InstantiateWeapon();
        // Setup input callbacks
        _playerActions.Attack.performed += Attack;
    }

    private void InstantiateWeapon()
    {
        currentWeapon = Instantiate(weaponSO.weaponModel, handPivot);
        currentWeapon.transform.forward = handPivotForward;
        currentWeapon.transform.position += handPivotOffset;

        Weapon weaponComponent = currentWeapon.AddComponent<Weapon>();
        weaponComponent?.Setup(weaponSO);
    }

    private void SwitchWeapon(WeaponSO newWeaponSO)
    {
        weaponSO = newWeaponSO;
        if (currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        InstantiateWeapon();
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
