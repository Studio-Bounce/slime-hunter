using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArea : MonoBehaviour
{
    [SerializeField] float jumpForce = 10.0f;
    [SerializeField] float jumpDuration = 0.5f;
    [Tooltip("Jump targets must be present outside this object's collider area")]
    [SerializeField] Transform[] jumpTargets;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply jump
            if (other.gameObject.TryGetComponent<PlayerController>(out var controller))
            {
                // Check which target is furthest, the player will jump there
                Vector3 target = other.transform.position;
                float maxDist = 0;
                foreach (Transform jumpTarget in jumpTargets)
                {
                    float distance = Vector3.Distance(other.gameObject.transform.position, jumpTarget.position);
                    if (distance > maxDist)
                    {
                        maxDist = distance;
                        target = jumpTarget.position;
                    }
                }

                controller.Jump(jumpForce, jumpDuration, target);
            }
        }
    }
}
