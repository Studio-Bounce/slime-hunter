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

    private bool isDashing = false;
    private bool isGrounded = true;
    private bool isJump = true;

    [Header("Camera Handling")]
    public Transform cameraTransform;
    public float rotationDuration = 0.3f;
    public float rotationIncrement = 45f;
    private bool isRotating = false;

    [Header("Grounded Checks")]
    public Vector3 boxCastSize = Vector3.one;
    public float boxCastYOffset = 0f;
    public float boxCastDistance = 1f;

    // Other
    // Currently only really using for gravity, consider removing?
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
        Debug.Assert(cameraTransform != null, "Can't find main camera");
    }

    void FixedUpdate()
    {
        Dash();
        MovePlayer();
        RotateCamera();
        Jump();
    }

    private void Attack()
    {

    }

    private void RotateCamera()
    {
        if (isRotating) return;

        if (_playerActions.Rotate.ReadValue<float>() < 0)
        {
            StartCoroutine(PerformCameraRotate(rotationIncrement));
        }
        else if (_playerActions.Rotate.ReadValue<float>() > 0)
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

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 dashDirection = rb.velocity.normalized;
            Vector3 dashTarget = transform.position + dashDirection * dashDistance;

            StartCoroutine(PerformDash(dashTarget));
        }
    }

    // Naive jump that triggers once on fall
    void Jump()
    {
        // Check is grounded
        RaycastHit hit;

        isGrounded = Physics.BoxCast(transform.position + new Vector3(0, boxCastYOffset, 0), boxCastSize / 2f, Vector3.down, out hit, Quaternion.identity, boxCastDistance);

        // Reset jump if grounded
        if (isGrounded) isJump = false;

        // Jump once when off ledge
        if (!isGrounded && !isJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJump = true;
        }
    }

    System.Collections.IEnumerator PerformDash(Vector3 target)
    {
        Func<float, float> EaseOut = t => 1f - Mathf.Pow(1f - t, 2);
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

    System.Collections.IEnumerator PerformCameraRotate(float angle)
    {
        Func<float, float> EaseOut = t => 1f - Mathf.Pow(1f - t, 3);

        isRotating = true;
        float startTime = Time.time;
        Quaternion startRotation = cameraTransform.localRotation;
        Quaternion targetRotation = Quaternion.Euler(startRotation.eulerAngles.x, startRotation.eulerAngles.y + angle, startRotation.eulerAngles.z);
        Debug.Log($"{cameraTransform.localRotation.eulerAngles}:{targetRotation.eulerAngles}");

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

        Vector3 startPos = new Vector3(transform.position.x, transform.position.y+boxCastYOffset, transform.position.z);

        Gizmos.DrawWireCube(startPos, new Vector3(boxCastSize.x, boxCastSize.y, boxCastSize.z));
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPos, startPos + Vector3.down*boxCastDistance);
    }
}
