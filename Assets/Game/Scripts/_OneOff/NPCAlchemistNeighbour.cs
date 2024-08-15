using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAlchemistNeighbour : MonoBehaviour
{
    readonly int IsTalkingHash = Animator.StringToHash("isTalking");

    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartTalking()
    {
        animator.SetBool(IsTalkingHash, true);
    }

    public void StopTalking()
    {
        animator.SetBool(IsTalkingHash, false);
    }
}
