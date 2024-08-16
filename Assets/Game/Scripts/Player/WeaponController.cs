using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Positioning")]
    [Tooltip("Assumes the weapons prefab handle is positioned at origin (0,0,0) and points towards Z+")]
    public Transform handPivot;
    public Vector3 handPivotOffset;
    public Vector3 handPivotForward;
    [NonSerialized] public WeaponSO[] availableWeapons = new WeaponSO[2];

    [Header("Animations/Visuals")]
    public WeaponTrail weaponTrail;
    public AnimationClip baseAttackClip;

    [Header("Weapon & Animation")]
    float specialAttackDuration = 4.0f;
    [HideInInspector, NonSerialized] public AttackState currentAttackState = AttackState.INACTIVE;
    [HideInInspector] public bool isPerformingSpecialAttack = false;
    private AnimatorOverrideController _overrideAnimatorController;
    private int _equippedWeaponIndex = 0;
    private GameObject _currentWeaponPrefab;
    private Animator _animator;

    private readonly int attackComboIntHash = Animator.StringToHash("AttackCombo");
    private readonly int specialAttackBoolHash = Animator.StringToHash("SpecialAttack");
    private readonly int attackSubStateHash = Animator.StringToHash("AttackSubState");
    private readonly int attackStateHash = Animator.StringToHash("Attack");
    private readonly int locomotionStateHash = Animator.StringToHash("Locomotion");
    private int _attackMoveIndex = 0;

    private InventoryManager inventoryManager;

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
        get {
            if (availableWeapons[_equippedWeaponIndex] == null)
            {
                return null;
            }
            return availableWeapons[_equippedWeaponIndex];
        }
        set { availableWeapons[_equippedWeaponIndex] = value; }
    }

    private void Awake()
    {
        _animator = GetComponent<PlayerController>()?.animator;
        Debug.Assert(_animator != null, "Requires an animator");
        Debug.Assert(handPivot != null, "Requires hand location for weapon");
        _overrideAnimatorController = new AnimatorOverrideController(_animator.runtimeAnimatorController);
        inventoryManager = InventoryManager.Instance;
        inventoryManager.OnEquippedWeaponsChanged += OnWeaponUpdate;
    }

    void Start()
    {
        // Spawn Initial Weapon
        InitializeHandPivot();
        OnWeaponUpdate(availableWeapons);
    }

    private void OnDestroy()
    {
        if (inventoryManager != null)
        {
            inventoryManager.OnEquippedWeaponsChanged -= OnWeaponUpdate;
        }
    }

    private void OnWeaponUpdate(WeaponSO[] weapons)
    {
        availableWeapons = weapons;

        if (CurrentWeapon == null) return;
        if (_currentWeaponPrefab != null) Destroy(_currentWeaponPrefab);
        InstantiateWeapon(CurrentWeapon);
        (UIManager.Instance.HUDMenu as HUDMenu).UpdateWeaponIcon(CurrentWeapon.icon);
    }

    private void Update()
    {
        _animator.speed = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 1 / Time.timeScale;
    }

    public void EnableAttack() { }
    public void DisableAttack() { }

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
    public void InstantiateWeapon(WeaponSO weaponSO)
    {
        if (weaponSO == null) return;

        weaponTrail.SetupWeaponSettings(weaponSO);
        if (weaponSO.weaponModel != null)
        {
            _currentWeaponPrefab = Instantiate(weaponSO.weaponModel, handPivot);
            _currentWeaponPrefab.transform.forward = handPivot.forward;
            _currentWeaponPrefab.transform.position += handPivotOffset;
        }
    }

    // Replace base attack animation with weapon animations
    private void SetupAttackAnimation(AttackMove move)
    {
        _overrideAnimatorController[baseAttackClip.name] = move.clip;
        _animator.runtimeAnimatorController = _overrideAnimatorController;
    }

    public void CycleWeapon(InputAction.CallbackContext context)
    {
        if (!IsInterruptable()) return;

        // Cycle equipped
        int nextWeaponIndex = _equippedWeaponIndex == availableWeapons.Length - 1 ?
            0 : _equippedWeaponIndex + 1;

        if (availableWeapons[nextWeaponIndex] == null) return;
        _equippedWeaponIndex = nextWeaponIndex;

        // Reset any existing combo
        ResetCombo();
        if (_currentWeaponPrefab != null)
        {
            Destroy(_currentWeaponPrefab);
        }
        InstantiateWeapon(CurrentWeapon);
        (UIManager.Instance.HUDMenu as HUDMenu).UpdateWeaponIcon(CurrentWeapon.icon);
    }

    public bool Attack(InputAction.CallbackContext context)
    {
        if (CurrentWeapon == null) return false;

        if (!InterruptAttack() || isPerformingSpecialAttack) return false;

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

        // CLEANUP NEEDED: Drop in for new combo animator setup
        int newAttackAnimCount = _attackMoveIndex+1;

        _attackMoveIndex = isFinalAttack ? 0 : _attackMoveIndex + 1;
        

        // Rotate player towards attack
        transform.forward = direction;
        weaponTrail.transform.forward = direction;

        // Start attack 
        //_animator.SetTrigger(attackStartTriggerHash);

        // CLEANUP NEEDED: Drop in for new combo animator setup
        _animator.SetInteger(attackComboIntHash, newAttackAnimCount);
        yield return new WaitForSecondsRealtime(move.animationDelay);

        currentAttackState = AttackState.ACTIVE;
        RuntimeManager.PlayOneShot(move.audioHitEvent);
        weaponTrail.Attack(move, isFinalAttack);
        yield return new WaitForSecondsRealtime(move.attackDuration);

        currentAttackState = AttackState.WIND_DOWN;
        yield return new WaitForSecondsRealtime(move.comboDuration);
        if (currentAttackState == AttackState.WIND_DOWN)
        {
            ResetCombo();
            currentAttackState = AttackState.INACTIVE;
        }
    }

    public void ResetCombo()
    {
        _attackMoveIndex = 0;
        // CLEANUP NEEDED: Drop in for new combo animator setup
        _animator.SetInteger(attackComboIntHash, _attackMoveIndex);
    }

    // Interrupt current attack animation
    public bool InterruptAttack()
    {
        if (IsInterruptable()) {
            AnimatorStateInfo currentStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextStateInfo = _animator.GetNextAnimatorStateInfo(0);
            //if (currentStateInfo.shortNameHash == attackStateHash)
            //{
            //    _animator.CrossFade(locomotionStateHash, 0.0f);
            //}

            // CLEANUP NEEDED: Drop in for new combo animator setup
            // Check if the animator is in the attack sub-state
            if (_animator.GetInteger(attackComboIntHash) == 3)
            {
                _animator.CrossFade(locomotionStateHash, 0.0f);
            }
            return true;
        }
        return false;
    }

    public void SpecialAttack(InputAction.CallbackContext context)
    {
        if ((GameManager.Instance.PlayerSpecialAttack < GameManager.Instance.PlayerMaxSpecialAttack) ||
            isPerformingSpecialAttack)
            return;

        AudioManager.Instance.SpecialAttackInstance.start();
        GameManager.Instance.PlayerSpecialAttack = 0.0f;

        StartCoroutine(PerformSpecialAttack());
    }

    IEnumerator PerformSpecialAttack()
    {
        isPerformingSpecialAttack = true;
        StartCoroutine(weaponTrail.RunSpecialAttack(specialAttackDuration));
        if (CurrentWeapon.attackMoves.Count > 0)
        {
            weaponTrail.SetWeaponProps(CurrentWeapon.attackMoves[0]);
        }
        _animator.SetBool(specialAttackBoolHash, true);

        float elapsed = 0.0f;
        while (elapsed < specialAttackDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            // Animation no longer rotates the player so we manually rotate
            // TODO: Hardcoded special attack rotation
            gameObject.transform.Rotate(0, -Time.unscaledDeltaTime*1000, 0);
            yield return null;
        }

        isPerformingSpecialAttack = false;
        _animator.SetBool(specialAttackBoolHash, false);
        AudioManager.Instance.SpecialAttackInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
