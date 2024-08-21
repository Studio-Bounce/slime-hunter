using FMODUnity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController), typeof(WeaponController))]
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
    public float slowDownMultiplierOnSpecialAttack = 0.3f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public int dashStaminaUse = 10;

    public bool useGravity = true;

    bool _isDashing = false;
    bool _isJumping = false;

    [Header("Camera Handling")]
    public float rotationDuration = 0.3f;
    public float rotationIncrement = 45f;
    public Vector2 rotationRange = Vector2.zero;
    private float _currentRotation = 0;
    private bool _isRotating = false;
    private float rotateDir = 0; // To cache readvalue input for queued input

    // Used for jump, dash, and gravity
    CharacterController characterController;
    WeaponController weaponController;
    SpellController spellController;
    Trail trail;

    private float lastDashTime = 0.0f;

    public bool IsJumping { get { return _isJumping; } }
    public bool IsDashing { get { return _isDashing; } }

    public event Action OnPlayerDashEvent = delegate { };


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        weaponController = GetComponent<WeaponController>();
        spellController = GetComponent<SpellController>();
        trail = GetComponent<Trail>();

        // Find the virual camera and tell it about the player
        CameraManager.Instance.SetCameraFollow(transform);
    }

    private void Update()
    {
        // TODO: Only works on players cloned with instantiate. Strangely not on ones placed in the scene
        animator.speed = Mathf.Approximately(Time.timeScale, 0f) ? 1f : 1 / Time.timeScale;

        // Gravity simulation
        if (characterController != null && characterController.enabled && useGravity)
        {
            characterController.Move(Physics.gravity * Time.deltaTime);
        }

        // Movement
        MovePlayer();
    }

    public bool RotateCamera(InputAction.CallbackContext context)
    {
        if (_isRotating) return false;

        float newDir = context.ReadValue<float>();
        if (newDir != 0f) rotateDir = newDir;

        float newRotation = _currentRotation + rotateDir * rotationIncrement;
        if (newRotation >= rotationRange.x && newRotation <= rotationRange.y)
        {
            _currentRotation = newRotation;
            StartCoroutine(PerformCameraRotate(-rotateDir * rotationIncrement));
        }
        else
        {
            StartCoroutine(PerformCameraRotateError(-rotateDir * 10));
        }
        return true;
    }


    // -------------------- Movement Mechanism --------------------
    public void MovePlayer()
    {
        if (_isDashing || _isJumping || !characterController.enabled) return;

        Vector2 moveInput = InputManager.Instance.Movement;
        // Set move animation based on input
        animator.SetFloat(blendSpeedHash, moveInput.magnitude);
        if (moveInput == Vector2.zero)
            return;

        // Interrupt any attack release animation
        weaponController.InterruptAttack();

        Vector3 moveDirection = CameraManager.Instance.DirectionToCameraForward(transform.position,moveInput);
        // Rotate the player to look at the movement direction
        float _moveSpeed = moveSpeed * GameManager.Instance.PlayerSpeedMultiplier;
        if (!weaponController.IsInterruptable() || spellController.isCasting)
        {
            _moveSpeed *= slowDownMultiplierOnAttack;
        }
        else if (weaponController.isPerformingSpecialAttack)
        {
            // Don't Rotate
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
            !weaponController.isPerformingSpecialAttack &&
            (Time.time > lastDashTime + dashCooldown) &&
            weaponController.IsDashInterruptable() &&
                GameManager.Instance.UseStamina(dashStaminaUse)
            )
        {
            weaponController.ResetCombo();
            weaponController.DashInterruptAttack();
            Vector3 dashDirection = CameraManager.Instance.DirectionToCameraForward(transform.position, InputManager.Instance.Movement);
            dashDirection = dashDirection == Vector3.zero ? transform.forward : dashDirection;

            // Rotate player to dash
            transform.forward = dashDirection;
            _isDashing = true;
            trail.InitiateTrail();
            StartCoroutine(PerformDash(dashDirection));
            OnPlayerDashEvent.Invoke();
            return true;
        }
        return false;
    }

    IEnumerator PerformDash(Vector3 dashDirection)
    {
        RuntimeManager.PlayOneShot(AudioManager.Config.dash);
        animator.SetBool(dashBoolash, true);
        _isDashing = true;

        float elapsedTime = 0.0f;

        // Initialize previous dash progress
        float previousDashProgress = 0.0f;

        while (elapsedTime < dashDuration)
        {
            // Increment elapsed time
            elapsedTime += Time.unscaledDeltaTime;
            float dashProgress = Mathf.Clamp01(elapsedTime / dashDuration);
            dashProgress = Easing.EaseOutCubic(dashProgress);

            // Check for collisions
            if (CheckForwardCollisions())
            {
                // Return stamina if dash was a failure (movement < 50%)
                if (dashProgress < 0.5f)
                {
                    GameManager.Instance.ReturnStamina(dashStaminaUse);
                }
                break;
            }

            // Calculate movement based on deltaTime and progress
            float distanceCovered = dashDistance * (dashProgress - previousDashProgress);
            characterController.Move(distanceCovered * dashDirection);

            // Update previous progress for next frame calculation
            previousDashProgress = dashProgress;

            yield return null;
        }

        // Ensure final dash state is applied
        _isDashing = false;
        animator.SetBool(dashBoolash, false);
        lastDashTime = Time.time;
    }

    bool CheckForwardCollisions()
    {
        // Dash detection happens at 3/4th of the player height
        if (Physics.Raycast(transform.position + characterController.height * 0.75f * Vector3.up, transform.forward, out _, 1.0f))
        {
            return true;
        }
        return false;
    }

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
            timeElapsed += Time.unscaledDeltaTime;
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

    IEnumerator PerformCameraRotateError(float angle)
    {
        _isRotating = true;
        float elapsed = 0;
        float duration = rotationDuration/2;
        Transform cameraTransform = CameraManager.ActiveCineCamera.transform;
        Quaternion startRotation = cameraTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + angle, startRotation.eulerAngles.z);

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            t = Easing.PingPong(t);
            cameraTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        cameraTransform.localRotation = startRotation;

        _isRotating = false;
    }

    IEnumerator PerformCameraRotate(float angle)
    {
        _isRotating = true;
        float elapsed = 0;
        Transform cameraTransform = CameraManager.ActiveCineCamera.transform;
        Quaternion startRotation =  cameraTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + angle, startRotation.eulerAngles.z);

        while (elapsed < rotationDuration)
        {
            float t = elapsed / rotationDuration;
            t = Easing.EaseOutCubic(t);
            cameraTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        cameraTransform.localRotation = targetRotation;

        _isRotating = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Dash direction
        float height = (characterController != null) ? characterController.height : 1.5f;
        DebugExtension.DrawArrow(transform.position + height * 0.5f * Vector3.up, transform.forward, Color.magenta);
    }
#endif
}
