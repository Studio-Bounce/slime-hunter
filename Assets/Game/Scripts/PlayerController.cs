using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    public Animator animator;
    private int blendSpeedHash;
    private PlayerInputActions _inputActions;
    private PlayerInputActions.PlayerActions _playerActions;

    public float maximumSpeed = 5f;
    public float rotationSpeed = 5f;
    public float moveSpeed = 5f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float jumpForce = 20f;
    private Vector2 moveInput = Vector2.zero;

    private bool isDashing = false;
    private bool isGrounded = false;
    private bool isJump = true;

    [Header("Camera Handling")]
    public Transform cameraTransform;
    public float rotationDuration = 0.3f;
    public float rotationIncrement = 45f;
    private bool isRotating = false;

    [Header("Grounded Checks")]
    public LayerMask groundLayer;
    public Vector3 overlapBoxSize = Vector3.one;
    public float boxYOffset = 0f;

    // Other
    // Currently using for jump and dash. Consider not using rb?
    private Rigidbody rb;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
        _playerActions = _inputActions.Player;

        rb = GetComponent<Rigidbody>();
        blendSpeedHash = Animator.StringToHash("blendSpeed");
    }

    void Start()
    {
        Debug.Assert(cameraTransform != null, "Missing camera transform");

        _playerActions.Dash.performed += Dash;
        _playerActions.Rotate.performed += RotateCamera;
    }

    void FixedUpdate()
    {
        MovePlayer();
        CheckJump();
    }

    private void RotateCamera(InputAction.CallbackContext context)
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

    void MovePlayer()
    {
        if (isDashing) return;
        // Get forward based on camera
        Vector3 cameraToPlayer = (transform.position - cameraTransform.position);
        Vector2 forwardDirection = new Vector2(cameraToPlayer.x, cameraToPlayer.z);
        forwardDirection.Normalize();
        Vector2 rightDirection = new Vector2(forwardDirection.y, -forwardDirection.x);

        // Input handling
        Vector2 moveInput = _playerActions.Move.ReadValue<Vector2>();

        // Calculate movement direction based on forward
        Vector2 direction = (forwardDirection * moveInput.y + rightDirection * moveInput.x).normalized;

        // Rotate the player to look at the movement direction
        if (direction != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
            //Quaternion toRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.y), Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        // Set move animation based on input
        animator.SetFloat(blendSpeedHash, direction.magnitude, 0.05f, Time.deltaTime);
    }

    private void OnAnimatorMove()
    {
        // Move the player based off of root motion
        Vector2 movement = moveSpeed * new Vector2(animator.deltaPosition.x, animator.deltaPosition.z);
        transform.position += new Vector3(movement.x, 0, movement.y);
    }

    void Dash(InputAction.CallbackContext context)
    {
        if (!isDashing)
        {
            Vector3 dashTarget = transform.position + transform.forward * dashDistance;

            StartCoroutine(PerformDash(dashTarget));
        }
    }

    System.Collections.IEnumerator PerformDash(Vector3 target)
    {
        isDashing = true;
        float startTime = Time.time;
        Vector3 startPosition = transform.position;

        while (Time.time < startTime + dashDuration)
        {

            float t = (Time.time - startTime) / dashDuration;
            rb.MovePosition(Vector3.Lerp(startPosition, target, t));
            yield return null;
        }

        isDashing = false;
    }

    // Naive jump that triggers once when nothing is below
    void CheckJump()
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJump = true;
        }
        
    }

    System.Collections.IEnumerator PerformCameraRotate(float angle)
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


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 startPos = new Vector3(transform.position.x, transform.position.y+boxYOffset, transform.position.z);
        Gizmos.DrawWireCube(startPos, new Vector3(overlapBoxSize.x, overlapBoxSize.y, overlapBoxSize.z));
    }
}
