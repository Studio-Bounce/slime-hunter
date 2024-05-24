using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    public Animator animator;
    private int blendSpeedHash;
    private int dashHash;

    //public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float jumpForce = 20f;

    private Vector2 moveDirection = Vector2.zero;
    private bool isDashing = false;
    private bool isGrounded = false;
    private bool isJump = true;

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

    // Used for jump, dash, and gravity
    CharacterController characterController;
    InputController inputController;

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
        Debug.Assert(cameraTransform != null, "Missing camera transform");
    }

    void FixedUpdate()
    {
        CheckJump();
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

    public void MovePlayer()
    {
        if (isDashing)
        {
            return;
        }

        Vector2 moveInput = inputController.movement;
        // Set move animation based on input
        animator.SetFloat(blendSpeedHash, moveInput.magnitude);
        if (moveInput == Vector2.zero)
            return;

        // Get forward based on camera
        Vector3 cameraToPlayer = (transform.position - cameraTransform.position);
        Vector2 forwardDirection = new Vector2(cameraToPlayer.x, cameraToPlayer.z);
        forwardDirection.Normalize();
        Vector2 rightDirection = new Vector2(forwardDirection.y, -forwardDirection.x);

        // Calculate movement direction based on forward
        moveDirection = (forwardDirection * moveInput.y + rightDirection * moveInput.x).normalized;

        // Rotate the player to look at the movement direction
        if (moveDirection != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.y), Vector3.up); // Snap
        }

        // Move
        characterController.Move(moveSpeed * Time.deltaTime * new Vector3(moveDirection.x, 0, moveDirection.y));
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!isDashing && Time.time > lastDashTime + dashCooldown)
        {
            StartCoroutine(PerformDash(transform.forward * dashDistance));
        }
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
            transform.position = startPosition + dashVector * dashProgress;
            yield return null;
        }
        
        isDashing = false;
        animator.SetBool(dashHash, isDashing);
        lastDashTime = Time.time;
    }

    // Naive jump that triggers once when nothing is below
    private void CheckJump()
    {
        // Check is grounded
        var groundCollisions = Physics.OverlapBox(transform.position + new Vector3(0, boxYOffset, 0), overlapBoxSize / 2f, Quaternion.identity, groundLayer);
        isGrounded = groundCollisions.Length > 0;

        // Reset jump if grounded
        if (isGrounded)
        {
            isJump = false;
        }

        // Jump once when off ledge
        if (!isGrounded && !isJump)
        {
            Jump();
        }
        
    }

    private void Jump()
    {
        characterController.Move(Vector3.up * jumpForce);
        isJump = true;
    }

    IEnumerator PerformCameraRotate(float angle)
    {
        Func<float, float> EaseOut = t => 1f - Mathf.Pow(1f - t, 3);

        isRotating = true;
        float startTime = Time.time;
        Quaternion startRotation = cameraTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + angle, startRotation.eulerAngles.z);

        while (Time.time < startTime + rotationDuration)
        {

            float t = (Time.time - startTime) / rotationDuration;
            t = EaseOut(t);
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
