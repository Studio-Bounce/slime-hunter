using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpArea : MonoBehaviour
{
    [SerializeField] float jumpForce = 10.0f;
    [SerializeField] float jumpDuration = 0.5f;
    
    public Transform[] jumpTargets;

    private void Start()
    {
        jumpTargets = new Transform[transform.childCount];
        int i = 0;
        foreach (Transform child in transform)
        {
            jumpTargets[i++] = child;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply jump
            if (other.gameObject.TryGetComponent<PlayerController>(out var controller))
            {
                // Check which target is closest, the player will jump there
                Vector3 target = other.transform.position;
                float minDist = 100000f;
                foreach (Transform jumpTarget in jumpTargets)
                {
                    float distance = Vector3.Distance(other.gameObject.transform.position, jumpTarget.position);
                    if (distance < minDist)
                    {
                        minDist = distance;
                        target = jumpTarget.position;
                    }
                }

                controller.Jump(jumpForce, jumpDuration, target);
            }
        }
    }
}
