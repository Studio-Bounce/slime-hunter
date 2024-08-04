using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Blank state -- dead men tell no tales
public class BasicSlime_Dead : BasicSlime_BaseState
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Debug.Log("Slime Death");
    }
}
