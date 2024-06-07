using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Trail : MonoBehaviour
{
    enum TrailType
    {
        PLAYER,
        SLIME
    };
    public float activeTime = 2f;

    [Header("Mesh")]
    public float meshfreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public GameObject model;
    [SerializeField] TrailType trailType = TrailType.PLAYER;
    [SerializeField] LayerMask ignoreLightingLayer;

    [Header("Shader")]
    public Material mat;
    public int alphaReductionSteps = 10;
    public readonly string shaderVarRef = "_Alpha";

    bool isTrailActive;
    bool isFirstShadow;

    PlayerController playerController;

    private void Start()
    {
        if (trailType == TrailType.PLAYER)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    public bool InitiateTrail()
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
        while (timeActive > 0 && (trailType == TrailType.SLIME || playerController.IsDashing()))
        {
            timeActive -= meshfreshRate;

            if (isFirstShadow)
            {
                // Avoid showing 1st shadow. It is generally at the player position itself -- looks bad
                isFirstShadow = false;
            }
            else
            {
                GameObject gObj = Instantiate(model, transform.position, transform.rotation);
                SceneManager.MoveGameObjectToScene(gObj, gameObject.scene);
                gObj.transform.localScale = gameObject.transform.localScale;
                Utils.SetLayerRecursively(gObj, 10);

                SkinnedMeshRenderer[] skinnedMeshRenderers = gObj.GetComponentsInChildren<SkinnedMeshRenderer>();

                if (skinnedMeshRenderers.Length > 0)
                {
                    // Apply trail material
                    if (trailType == TrailType.PLAYER)
                    {
                        SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRenderers[skinnedMeshRenderers.Length - 1];

                        Material[] trailMats = { mat, mat, mat, mat, mat };
                        skinnedMeshRenderer.materials = trailMats;

                        StartCoroutine(AnimateMaterialFloat(skinnedMeshRenderer));
                    }
                    else if (trailType == TrailType.SLIME)
                    {
                        // Disable the animator on model
                        if (gObj.TryGetComponent<Animator>(out Animator modelAnimator))
                        {
                            modelAnimator.enabled = false;
                        }

                        Material[] trailMats = { mat };
                        // Apply trail material to outer body
                        skinnedMeshRenderers[0].materials = trailMats;
                        StartCoroutine(AnimateMaterialFloat(skinnedMeshRenderers[0]));
                        // Disable any other mesh renderers
                        for (int i = 1; i < skinnedMeshRenderers.Length; i++)
                        {
                            skinnedMeshRenderers[i].gameObject.SetActive(false);
                        }
                    }

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
