using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberSlime_Explode : BasicSlime_BaseState
{
    BomberSlime_FSM bFSM;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bFSM = (BomberSlime_FSM)fsm;

        // Disable behaviours
        bFSM.seekSteeringBehaviour.enabled = false;
        bFSM.seekSteeringBehaviour.gameObject.SetActive(false);
        bFSM.wanderSteeringBehaviour.enabled = false;
        bFSM.wanderSteeringBehaviour.gameObject.SetActive(false);
        bFSM.slimeAgent.reachedGoal = true;
        bFSM.slimeAgent.velocity = Vector3.zero;

        // Change slime material (color)
        if (bFSM.slimeOuterMesh.materials.Length > 0)
        {
            Material redGlow = bFSM.attackMat;
            // Ensure the material has an emission property
            if (redGlow.HasProperty("_EmissionColor"))
            {
                // Enable the emission keyword
                redGlow.EnableKeyword("_EMISSION");

                // Set the HDR emission color
                Color finalEmissionColor = redGlow.color;
                redGlow.SetColor("_EmissionColor", finalEmissionColor);
            }
            Material[] mats = { redGlow };
            bFSM.slimeOuterMesh.materials = mats;
        }
        // Disable shadow in slime outer mesh to show transparent material properly
        bFSM.slimeOuterMesh.gameObject.layer = GameConstants.IgnoreLightingLayer;

        bFSM.Explode();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Die
        bFSM.ChangeState(bFSM.DeadStateName);
    }
}
