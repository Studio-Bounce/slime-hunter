using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSteeringAgent : SteeringAgent
{
    [Header("Slime Attributes")]
    public bool beBouncy = false;
    public float bounceForce = 5.0f;

    private readonly int IsMoving = Animator.StringToHash("isMoving");

    protected override void Update()
    {
        base.Update();
        ComputeYMovement();
        
        if (beBouncy)
        {
            animator.SetBool(IsMoving, velocity.magnitude != 0);
        }
    }

    private void ComputeYMovement()
    {
        if (beBouncy && velocity.magnitude > 0)
        {
#if UNITY_EDITOR
            if (characterController == null || !characterController.enabled)
            {
                Debug.LogWarning("No character controller. Can not be bouncy!");
            }
#endif

            // Shoot it up
            if (characterController.isGrounded)
            {
                upVelocity += bounceForce;
            }
        }
    }
}
