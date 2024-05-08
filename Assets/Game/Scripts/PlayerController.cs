using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this to change movement speed
    public float dashDistance = 5f; // Adjust this to change dash distance
    public float dashDuration = 0.2f; // Adjust this to change dash duration

    private Rigidbody rb;
    private bool isDashing = false;

    private bool isGrounded = true;
    private bool isJump = false;
    public float groundDistanceCheck = 0.1f;
    public float jumpForce = 5f;

    void Start()
    {
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
        CheckJump();
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

        isGrounded = Physics.Raycast(transform.position, -Vector3.up, out hit, groundDistanceCheck);
  
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
}
