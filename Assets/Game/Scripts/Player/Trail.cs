using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Trail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh")]
    public float meshfreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public GameObject ghostPlayer;

    [Header("Shader")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate;
    public float shaderVarRefreshrate = 0.05f;


    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    
    void Update()
    {
        if(Input.GetKeyDown (KeyCode.Space) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
            
        }
    }

    IEnumerator ActivateTrail (float timeActive)
    {
        while (timeActive > 0) 
        {
            timeActive -= meshfreshRate;

            if(skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i=0; i<skinnedMeshRenderers.Length; i++)
            {
                //GameObject gObj = new GameObject();
                //gObj.transform.SetPositionAndRotation(PositionToSpawn.position, PositionToSpawn.rotation);

                //MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                //MeshFilter mf = gObj.AddComponent<MeshFilter>();

                //Mesh mesh = new Mesh();
                //skinnedMeshRenderers[i].BakeMesh(mesh);

                //mf.mesh = mesh;
                //mr.material = mat;
                GameObject gObj = Instantiate(ghostPlayer, transform.position, transform.rotation);
                SkinnedMeshRenderer skinnedMeshRenderer = gObj.GetComponentInChildren<SkinnedMeshRenderer>();

                StartCoroutine(AnimateMaterialFloat(skinnedMeshRenderer, 0, shaderVarRate, shaderVarRefreshrate));

                Destroy(gObj, meshDestroyDelay);
            }

            yield return new WaitForSeconds(meshfreshRate);
        
        }

        isTrailActive = false;
    }
    IEnumerator AnimateMaterialFloat (SkinnedMeshRenderer skinnedMeshRenderer, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
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
