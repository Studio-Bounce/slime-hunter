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
    public AnimationCurve animationSpeedCurve;
    [Tooltip("A GameObject that has a Visual Effect component")]
    public VisualEffect weaponVFX;
    public AnimationClip baseAttackClip;

    Weapon _weaponComponent;
    private AnimatorOverrideController _overrideAnimatorController;
    private int _equippedWeaponIndex = 0;
    private GameObject _currentWeaponPrefab;
    private Animator _animator;
    private readonly int attackTriggerHash = Animator.StringToHash("Attack");
    private readonly int attackStateHash = Animator.StringToHash("Attack");


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _overrideAnimatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
    }

    void Start()
    {
        Debug.Assert(handPivot != null, "Requires hand location for weapon");

        // Spawn Initial Weapon
        InitializeHandPivot();
        InstantiateWeapon(availableWeapons[_equippedWeaponIndex]);
    }

    private void Update()
    {
        _animator.speed = 1.0f;
        if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == attackStateHash)
        {
            float animationTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            _animator.speed = animationSpeedCurve.Evaluate(animationTime);
        }
    }

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

    // WIP: Should instantiate all weapons to begin with and disable as needed
    private void InstantiateWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;
        _currentWeaponPrefab = Instantiate(weaponSO.weaponModel, handPivot);
        _currentWeaponPrefab.transform.forward = handPivot.forward;
        _currentWeaponPrefab.transform.position += handPivotOffset;
        _weaponComponent = _currentWeaponPrefab.AddComponent<Weapon>( );
        _weaponComponent?.Setup(weaponSO);
        _weaponComponent.active = false;
        SetupWeaponAnimations(weaponSO);
    }

    // Replace base attack animation with weapon animations
    private void SetupWeaponAnimations(WeaponSO weaponSO)
    {
        foreach (AnimationClip move in weaponSO.attackMoves)
        {
            _overrideAnimatorController[baseAttackClip.name] = move;
        }
        _animator.runtimeAnimatorController = _overrideAnimatorController;
    }

    private AnimationClip GetAnimationClipByName(RuntimeAnimatorController controller, string clipName)
    {
        foreach (AnimationClip clip in controller.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }

    public void CycleWeapon(InputAction.CallbackContext context)
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

    public void Attack(InputAction.CallbackContext context)
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
        //StartCoroutine(ApplyAttackCurve());
    }

    public void EnableAttack()
    {
        weaponVFX.transform.position = handPivot.transform.position;
        weaponVFX.transform.forward = handPivotForward;
        PlayAttackVFX(availableWeapons[_equippedWeaponIndex]);
        _weaponComponent.active = true;
    }
    public void DisableAttack()
    {
        _weaponComponent.active = false;
    }

    private void PlayAttackVFX(WeaponSO weaponSO)
    {
        weaponVFX.Play();
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

        // Weapon Attack
        Handles.color = new Color(1, 0, 1, 0.1f);
        if (availableWeapons[0] != null)
        {
            float attackAngle = 90f;
            Vector3 attackStart = Quaternion.AngleAxis(-attackAngle / 2, transform.up) * transform.forward;
            Handles.DrawSolidArc(transform.position, transform.up, attackStart, attackAngle, availableWeapons[0].range);
        }
    }
#endif
}
