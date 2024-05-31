using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Trail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh")]
    public float meshfreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public GameObject playerModel;

    [Header("Shader")]
    public Material mat;
    public int alphaReductionSteps = 10;
    public readonly string shaderVarRef = "_Alpha";

    bool isTrailActive;
    bool isFirstShadow;

    public bool InitiateTrail(InputAction.CallbackContext context)
    {
        if (!isTrailActive)
        {
            isTrailActive = true;
            isFirstShadow = true;
            StartCoroutine(ActivateTrail(activeTime));
            return true;
        }
        return false;
    }

    IEnumerator ActivateTrail (float timeActive)
    {
        while (timeActive > 0) 
        {
            timeActive -= meshfreshRate;

            if (isFirstShadow)
            {
                // Avoid showing 1st shadow. It is generally at the player position itself -- looks bad
                isFirstShadow = false;
            }
            else
            {
                GameObject gObj = Instantiate(playerModel, transform.position, transform.rotation);

                SkinnedMeshRenderer[] skinnedMeshRenderers = gObj.GetComponentsInChildren<SkinnedMeshRenderer>();

                if (skinnedMeshRenderers.Length > 0)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRenderers[skinnedMeshRenderers.Length - 1];

                    // Apply trail material
                    Material[] trailMats = { mat, mat, mat, mat, mat };
                    skinnedMeshRenderer.materials = trailMats;

                    StartCoroutine(AnimateMaterialFloat(skinnedMeshRenderer));
                }

                Destroy(gObj, meshDestroyDelay + 0.01f);
            }

            yield return new WaitForSeconds(meshfreshRate);
        
        }

        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat (SkinnedMeshRenderer skinnedMeshRenderer)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);
        // The alpha value will be reduced in alphaReductionSteps steps
        float goal = 0;
        float rate = valueToAnimate / alphaReductionSteps;
        float refreshRate = meshDestroyDelay / alphaReductionSteps;

        while (valueToAnimate > goal && skinnedMeshRenderer != null && skinnedMeshRenderer.gameObject != null)
        {
            valueToAnimate -= rate;
            foreach (Material mat in skinnedMeshRenderer.materials)
            {
                mat.SetFloat(shaderVarRef, valueToAnimate);
            }
            yield return new WaitForSeconds (refreshRate);
        }
    }
}
