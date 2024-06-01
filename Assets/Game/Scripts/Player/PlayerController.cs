using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

[RequireComponent(typeof(Animator), typeof(CharacterController), typeof(WeaponController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    public Animator animator;
    private int blendSpeedHash;
    private int dashHash;

    //public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float slowDownMultiplierOnAttack = 0.3f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;

    private bool isDashing = false;
    private bool isGrounded = false;
    private bool isJumping = false;

    [Header("Camera Handling")]
    public Transform cameraTransform;
    public float rotationDuration = 0.3f;
    public float rotationIncrement = 45f;
    private bool isRotating = false;

    [Header("Grounded Checks")]
    public bool useGravity = true;
    public LayerMask groundLayer;
    public Vector3 overlapBoxSize = Vector3.one;
    public float boxYOffset = 0f;
    private float yVelocity = 0f;

    // Used for jump, dash, and gravity
    CharacterController characterController;
    InputController inputController;
    WeaponController weaponController;

    private float lastDashTime = 0.0f;

    private void Awake()
    {
        blendSpeedHash = Animator.StringToHash("blendSpeed");
        dashHash = Animator.StringToHash("isDashing");
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        inputController = GetComponent<InputController>();
        weaponController = GetComponent<WeaponController>();

        Debug.Assert(cameraTransform != null, "Missing camera transform");
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;
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
        if (isRotating) return;

        float rotateDir = context.ReadValue<float>();

        if (rotateDir < 0)
        {
            StartCoroutine(PerformCameraRotate(rotationIncrement));
        }
        else if (rotateDir > 0)
        {
            StartCoroutine(PerformCameraRotate(-rotationIncrement));
        }

    }

    // -------------------- Movement Mechanism --------------------
    public void MovePlayer()
    {
        if (isDashing || isJumping) return;

        Vector2 moveInput = inputController.movement;
        // Set move animation based on input
        animator.SetFloat(blendSpeedHash, moveInput.magnitude);
        if (moveInput == Vector2.zero)
            return;

        // Interrupt any attack release animation
        weaponController.InterruptAttack();

        Vector3 moveDirection = Utils.DirectionToCameraForward(transform.position,moveInput);
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
        if (!isDashing && Time.time > lastDashTime + dashCooldown && weaponController.IsInterruptable())
        {
            weaponController.ResetCombo();
            Vector3 dashDirection = Utils.DirectionToCameraForward(transform.position, inputController.movement);
            dashDirection = dashDirection == Vector3.zero ? transform.forward : dashDirection;

            StartCoroutine(PerformDash(dashDirection * dashDistance));
            return true;
        }
        return false;
    }

    IEnumerator PerformDash(Vector3 dashVector)
    {
        isDashing = true;
        animator.SetBool(dashHash, isDashing);

        // Dash follows the curve of y^3 = x from 0 to 1
        // Provides a quick action in beginning which then slows
        float dashProgress = 0.0f;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        while (dashProgress <= 1.0f)
        {
            dashProgress = (Time.time - startTime) / dashDuration;

            dashProgress = Easing.EaseOutCubic(dashProgress);

            transform.position = startPosition + dashVector * dashProgress;
            yield return null;
        }
        
        isDashing = false;
        animator.SetBool(dashHash, isDashing);
        lastDashTime = Time.time;
    }

    // -------------------- Jump Mechanism --------------------

    public void Jump(float upForce, float jumpDuration, Vector3 target)
    {
        if (!isJumping)
        {
            // Orient towards target
            transform.LookAt(target);
            StartCoroutine(SmoothJump(upForce, jumpDuration, target));
        }
    }
    IEnumerator SmoothJump(float upForce, float jumpDuration, Vector3 target)
    {
        isJumping = true;
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
        isJumping = false;
    }

    IEnumerator PerformCameraRotate(float angle)
    {
        isRotating = true;
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

        isRotating = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y+boxYOffset, transform.position.z);
        Gizmos.DrawWireCube(startPos, new Vector3(overlapBoxSize.x, overlapBoxSize.y, overlapBoxSize.z));

        // Dash direction
        DebugExtension.DrawArrow(transform.position, transform.forward, Color.blue);
    }
#endif
}
