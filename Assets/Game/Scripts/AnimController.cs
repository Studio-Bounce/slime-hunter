using UnityEngine;

public class AnimController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get reference to the Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Check for input to trigger dash animation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Trigger dash animation
            animator.SetTrigger("Dash");
        }

        // Check for input to trigger attack animation
        if (Input.GetMouseButtonDown(0)) // Assuming left mouse button for attack
        {
            // Trigger attack animation
            animator.SetTrigger("Attack");
        }
    }
}
