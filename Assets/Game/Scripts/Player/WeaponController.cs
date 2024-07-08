using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.VFX;

[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    [Header("Positioning")]
    [Tooltip("Assumes the weapons prefab handle is at positioned at origin (0,0,0) and point towards Z+")]
    public Transform handPivot;
    public Vector3 handPivotOffset;
    public Vector3 handPivotForward;
    public WeaponSO[] availableWeapons = new WeaponSO[3];

    [Header("Animations/Visuals")]
    public WeaponTrail weaponTrail;
    public AnimationClip baseAttackClip;

    // Weapon&Animation
    [HideInInspector, NonSerialized] public AttackState currentAttackState = AttackState.INACTIVE;
    private AnimatorOverrideController _overrideAnimatorController;
    private int _equippedWeaponIndex = 0;
    private GameObject _currentWeaponPrefab;
    private Animator _animator;

    private readonly int attackStartTriggerHash = Animator.StringToHash("AttackStart");
    private readonly int attackEndTriggerHash = Animator.StringToHash("AttackEnd");
    private readonly int attackStateHash = Animator.StringToHash("Attack");
    private readonly int baseStateHash = Animator.StringToHash("Locomotion");
    private int _attackMoveIndex = 0;

    public enum AttackState
    {
        WIND_UP,
        ACTIVE,
        WIND_DOWN, // This state should be interruptable
        INACTIVE
    }

    public bool IsAttack()
    {
        return currentAttackState != AttackState.INACTIVE;
    }

    public bool IsInterruptable()
    {
        return currentAttackState == AttackState.INACTIVE ||
            currentAttackState == AttackState.WIND_DOWN;
    }

    public bool IsDashInterruptable()
    {
        return currentAttackState != AttackState.WIND_UP;
    }

    public WeaponSO CurrentWeapon
    {
        get { return availableWeapons[_equippedWeaponIndex]; }
        set { availableWeapons[_equippedWeaponIndex] = value; }
    }

    private void Awake()
    {

        _animator = GetComponent<PlayerController>()?.animator;
        Debug.Assert(_animator != null);
        _overrideAnimatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
    }

    void Start()
    {
        Debug.Assert(handPivot != null, "Requires hand location for weapon");

        // Spawn Initial Weapon
        InitializeHandPivot();
        InstantiateWeapon(CurrentWeapon);
    }

    private void Update()
    {
        _animator.speed = 1.0f;
    }

    public void EnableAttack() {}
    public void DisableAttack() {}

    // Creates a new child GameObject to use as pivot transform so we don't influence the original hand rotation
    // This is incase you pick the hand bone itself to be the pivot
    private void InitializeHandPivot()
    {
        GameObject newPivot = new GameObject("HandPivot");
        newPivot.transform.SetParent(handPivot.transform);
        newPivot.transform.localPosition = Vector3.zero;
        handPivot = newPivot.transform;
        handPivot.forward = handPivotForward;
    }

    // TODO: Should pool all weapons to begin with and disable as needed
    private void InstantiateWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;
        _currentWeaponPrefab = Instantiate(weaponSO.weaponModel, handPivot);
        _currentWeaponPrefab.transform.forward = handPivot.forward;
        _currentWeaponPrefab.transform.position += handPivotOffset;
        // The weapon trail needs to know current weapon's settings
        weaponTrail.SetupWeaponSettings(weaponSO);
    }

    // Replace base attack animation with weapon animations
    private void SetupAttackAnimation(AttackMove move)
    {
        _overrideAnimatorController[baseAttackClip.name] = move.clip;
        _animator.runtimeAnimatorController = _overrideAnimatorController;
    }

    public void CycleWeapon(InputAction.CallbackContext context)
    {
        // Cycle equipped
        _equippedWeaponIndex = _equippedWeaponIndex == availableWeapons.Length-1 ? 
            0 : _equippedWeaponIndex+1;
        // Reset any existing combo
        _attackMoveIndex = 0;

        if (_currentWeaponPrefab != null)
        {
            Destroy(_currentWeaponPrefab);
        }

        InstantiateWeapon(CurrentWeapon);
    }

    public bool Attack(InputAction.CallbackContext context)
    {
        if (!InterruptAttack()) return false;

        // Get vector from player to mouse click
        Vector2 clickPosition = Mouse.current.position.ReadValue();
        Vector2 currentScreenPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 clickDirection = (clickPosition - currentScreenPos).normalized;

        // Align to camera forward
        Vector3 attackDirection = CameraManager.Instance.DirectionToCameraForward(transform.position, clickDirection);
        StartCoroutine(PerformAttack(CurrentWeapon.attackMoves[_attackMoveIndex], attackDirection));

        return true;
    }

    private IEnumerator PerformAttack(AttackMove move, Vector3 direction)
    {
        currentAttackState = AttackState.WIND_UP;
        SetupAttackAnimation(move);

        // Increment combo
        bool isFinalAttack = (_attackMoveIndex == CurrentWeapon.attackMoves.Count - 1);
        _attackMoveIndex = isFinalAttack ? 0 : _attackMoveIndex + 1;

        // Rotate player towards attack
        transform.forward = direction;
        weaponTrail.transform.forward = direction;

        // Start attack 
        _animator.SetTrigger(attackStartTriggerHash);
        yield return new WaitForSeconds(move.attackDelay);

        currentAttackState = AttackState.ACTIVE;
        weaponTrail.Attack(move, isFinalAttack);
        yield return new WaitForSeconds(move.duration);

        currentAttackState = AttackState.WIND_DOWN;
        yield return new WaitForSeconds(move.clip.length - (move.attackDelay + move.duration));
        if (currentAttackState == AttackState.WIND_DOWN)
        {
            _attackMoveIndex = 0;
            currentAttackState = AttackState.INACTIVE;
        }
    }

    public void ResetCombo()
    {
        _attackMoveIndex = 0;
    }

    // Interrupt current attack animation
    public bool InterruptAttack()
    {
        if (IsInterruptable()) {
            if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == attackStateHash)
            {
                _animator.CrossFade(baseStateHash, 0.0f);
            }
            return true;
        }
        return false;
    }

    public bool DashInterruptAttack()
    {
        if (IsDashInterruptable())
        {
            weaponTrail.Deactivate();
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Weapon Positioning
        Gizmos.color = new Color(0, 0, 1);

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

        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }
#endif
}
