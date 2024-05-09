using System;
using System.ComponentModel;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Properties")]
    public float moveSpeed = 5f; // Adjust this to change movement speed
    public float dashDistance = 5f; // Adjust this to change dash distance
    public float dashDuration = 0.2f; // Adjust this to change dash duration
    public float jumpForce = 20f;

    private Rigidbody rb;
    [ReadOnly] public bool isDashing = false;
    [ReadOnly] public bool isGrounded = true;
    [ReadOnly] public bool isJump = false;

    [Header("Camera Handling")]
    [ReadOnly] public bool isRotating = false;
    public Transform cameraTransform;
    public float rotationDuration = 0.3f;
    public float rotationIncrement = 45f;

    [Header("Grounded Checks")]
    public Vector3 boxCastSize = Vector3.one;
    public float boxCastDistance = 1f;

    void Start()
    {
        Debug.Assert(cameraTransform != null);
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isDashing)
        {
            MovePlayer();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Dash();
            }
        }
        RotateCamera();
        CheckJump();
    }

    private void RotateCamera()
    {
        if (isRotating) return;
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(PerformCameraRotate(rotationIncrement));
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(PerformCameraRotate(-rotationIncrement));
        }
        
    }

    void MovePlayer()
    {
        // Input handling
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector2 direction = new Vector3(horizontalInput, verticalInput).normalized;

        // Move the player
        Vector2 movement = moveSpeed * direction;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.y);
    }

    void Dash()
    {
        Vector3 dashDirection = rb.velocity.normalized;
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;

        StartCoroutine(PerformDash(dashTarget));
    }

    // Naive jump that triggers once on fall
    void CheckJump()
    {
        // Check is grounded
        RaycastHit hit;

        isGrounded = Physics.BoxCast(transform.position, boxCastSize / 2f, Vector3.down, out hit, Quaternion.identity, boxCastDistance);
  
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

        Vector3 startPos = new Vector3(transform.position.x, transform.position.y-boxCastDistance / 2f, transform.position.z);

        Gizmos.DrawWireCube(startPos, new Vector3(boxCastSize.x, boxCastDistance, boxCastSize.z));
    }
}
