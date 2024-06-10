using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(Animator), typeof(CharacterController), typeof(WeaponController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    public Animator animator;
    private int blendSpeedHash = Animator.StringToHash("blendSpeed");
    private int dashBoolash = Animator.StringToHash("isDashing");
    private int jumpTriggerHash = Animator.StringToHash("Jump");

    //public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.5f;
    public float slowDownMultiplierOnAttack = 0.3f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public int dashStaminaUse = 10;

    public bool useGravity = true;

    bool _isDashing = false;
    bool _isJumping = false;

    [Header("Camera Handling")]
    public Transform cameraTransform;
    public float rotationDuration = 0.3f;
    public float rotationIncrement = 45f;
    public Vector2 rotationRange = Vector2.zero;
    private float _currentRotation = 0;
    private bool _isRotating = false;

    // Used for jump, dash, and gravity
    CharacterController characterController;
    InputController inputController;
    WeaponController weaponController;
    Trail trail;

    private float lastDashTime = 0.0f;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputController = GetComponent<InputController>();
        weaponController = GetComponent<WeaponController>();
        trail = GetComponent<Trail>();

        Debug.Assert(cameraTransform != null, "Missing camera transform");
    }

    private void Update()
    {
        // Gravity simulation
        if (characterController != null && characterController.enabled && useGravity)
        {
            characterController.Move(Physics.gravity * Time.deltaTime);
        }

        // Movement
        MovePlayer();
    }

    public void RotateCamera(InputAction.CallbackContext context)
    {
        if (_isRotating) return;

        float rotateDir = context.ReadValue<float>();

        if (rotateDir < 0 && _currentRotation > rotationRange.x)
        {
            _currentRotation -= rotationIncrement;
            StartCoroutine(PerformCameraRotate(rotationIncrement));
        }
        else if (rotateDir > 0 && _currentRotation < rotationRange.y)
        {
            _currentRotation += rotationIncrement;
            StartCoroutine(PerformCameraRotate(-rotationIncrement));
        }

    }

    // -------------------- Movement Mechanism --------------------
    public void MovePlayer()
    {
        if (_isDashing || _isJumping) return;

        Vector2 moveInput = inputController.movement;
        // Set move animation based on input
        animator.SetFloat(blendSpeedHash, moveInput.magnitude);
        if (moveInput == Vector2.zero)
            return;

        // Interrupt any attack release animation
        weaponController.InterruptAttack();

        Vector3 moveDirection = CameraManager.Instance.DirectionToCameraForward(transform.position,moveInput);
        // Rotate the player to look at the movement direction
        float _moveSpeed = moveSpeed;
        if (!weaponController.IsInterruptable())
        {
            _moveSpeed *= slowDownMultiplierOnAttack;
        } 
        else if (moveDirection != Vector3.zero) // Only rotate to movement when not attacking
        {
            transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.up); // Snap
        }

        // Move
        characterController.Move(_moveSpeed * Time.deltaTime * moveDirection);
    }

    // -------------------- Dash Mechanism --------------------
    public bool Dash(InputAction.CallbackContext context)
    {
        // Dashing while jumping is not allowed
        if (!_isDashing &&
            !_isJumping &&
            (Time.time > lastDashTime + dashCooldown) &&
            weaponController.IsDashInterruptable() &&
            (GameManager.Instance.PlayerStamina >= dashStaminaUse))
        {
            weaponController.ResetCombo();
            weaponController.DashInterruptAttack();
            Vector3 dashDirection = CameraManager.Instance.DirectionToCameraForward(transform.position, inputController.movement);
            dashDirection = dashDirection == Vector3.zero ? transform.forward : dashDirection;

            // Rotate player to dash
            transform.forward = dashDirection;
            _isDashing = true;
            trail.InitiateTrail();
            StartCoroutine(PerformDash(dashDirection * dashDistance));

            return true;
        }
        return false;
    }

    IEnumerator PerformDash(Vector3 dashVector)
    {
        animator.SetBool(dashBoolash, _isDashing);

        // Dash follows the curve of y^3 = x from 0 to 1
        // Provides a quick action in beginning which then slows
        float dashProgress = 0.0f;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        // Dash shouldn't use stamina if dash is not possible. Hence, this check.
        bool staminaUsed = false;

        while (dashProgress <= 1.0f)
        {
            dashProgress = (Time.time - startTime) / dashDuration;
            dashProgress = Easing.EaseOutCubic(dashProgress);

            // Dash is very quick. As a result, it can pass through colliders
            // We need to detect and stop dash if this happens
            if (CheckForwardCollisions())
            {
                break;
            }
            else if (!staminaUsed)
            {
                staminaUsed = true;
                // Stamina might have been changed by some other action.
                // Just confirm that player has enough stamina for dashing
                if (GameManager.Instance.PlayerStamina < dashStaminaUse) break;
                GameManager.Instance.UseStamina(dashStaminaUse);
            }

            transform.position = startPosition + dashVector * dashProgress;

            yield return null;
        }
        
        _isDashing = false;
        animator.SetBool(dashBoolash, _isDashing);
        lastDashTime = Time.time;
    }

    bool CheckForwardCollisions()
    {
        // Dash detection happens at 1/3rd of the player height
        if (Physics.Raycast(transform.position + characterController.height * 0.33f * Vector3.up, transform.forward, out _, 1.0f))
        {
            return true;
        }
        return false;
    }

    public bool IsDashing() { return _isDashing; }

    // -------------------- Jump Mechanism --------------------

    public void Jump(float upForce, float jumpDuration, Vector3 target)
    {
        // Jumping while dashing is not allowed
        if (!_isJumping && !_isDashing)
        {
            // Orient towards target
            transform.LookAt(target);
            StartCoroutine(SmoothJump(upForce, jumpDuration, target));
        }
    }
    IEnumerator SmoothJump(float upForce, float jumpDuration, Vector3 target)
    {
        animator.SetTrigger(jumpTriggerHash);

        _isJumping = true;
        useGravity = false;
        float timeElapsed = 0f;
        Vector3 initialPosition = transform.position;

        while (timeElapsed < jumpDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / jumpDuration;

            Vector3 currentPosition = Vector3.Lerp(initialPosition, target, t);
            // Calculate the current height using a parabolic curve
            float currentHeight = Mathf.Lerp(0, upForce, t) - (t * t * upForce);

            Vector3 moveDirection = (currentPosition - transform.position) + Vector3.up * currentHeight;
            characterController.Move(moveDirection);

            yield return null;
        }

        useGravity = true;
        _isJumping = false;
    }

    IEnumerator PerformCameraRotate(float angle)
    {
        _isRotating = true;
        float startTime = Time.time;
        Quaternion startRotation = cameraTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + angle, startRotation.eulerAngles.z);

        while (Time.time < startTime + rotationDuration)
        {

            float t = (Time.time - startTime) / rotationDuration;
            t = Easing.EaseOutCubic(t);
            cameraTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }
        cameraTransform.localRotation = targetRotation;

        _isRotating = false;
    }

    public bool IsJumping() { return _isJumping; }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Dash direction
        float height = (characterController != null) ? characterController.height : 1.5f;
        DebugExtension.DrawArrow(transform.position + height * 0.33f * Vector3.up, transform.forward, Color.magenta);
    }
#endif
}
