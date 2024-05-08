using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this to change movement speed
    public float dashDistance = 5f; // Adjust this to change dash distance
    public float dashDuration = 0.2f; // Adjust this to change dash duration

    private Rigidbody rb;
    private bool isDashing = false;

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
    }

    void MovePlayer()
    {
        // Input handling
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement direction
        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // Move the player
        rb.velocity = movement * moveSpeed;
    }

    void Dash()
    {
        Vector3 dashDirection = rb.velocity.normalized;
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;

        StartCoroutine(PerformDash(dashTarget));
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
