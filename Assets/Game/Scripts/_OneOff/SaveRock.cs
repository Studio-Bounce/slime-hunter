using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveRock : MonoBehaviour
{
    public bool emitLight = false;
    public Color emissionColor = Color.white;
    public float emissionIntensity = 3f;
    public float timetaken = 1f;

    bool isEmitting = false;
    MeshRenderer mr;

    private void Start()
    {
        emitLight = false;
        isEmitting = true;
        mr = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (emitLight && !isEmitting)
        {
            // Light up
            isEmitting = true;

            StartCoroutine(EmitLight());
        }
        else if (!emitLight && isEmitting)
        {
            // Turn light off
            isEmitting = false;

            mr.material.EnableKeyword("_EMISSION");
            mr.material.SetColor("_EmissionColor", Color.black * Mathf.LinearToGammaSpace(0));
        }
    }

    IEnumerator EmitLight()
    {
        float timeElapsed = 0f;
        while (timeElapsed < timetaken)
        {
            timeElapsed += Time.deltaTime;

            float emitIntensity = (timeElapsed / timetaken) * emissionIntensity;
            mr.material.EnableKeyword("_EMISSION");
            mr.material.SetColor("_EmissionColor", emissionColor * Mathf.LinearToGammaSpace(emitIntensity));

            yield return null;
        }
        mr.material.SetColor("_EmissionColor", emissionColor * Mathf.LinearToGammaSpace(emissionIntensity));
    }
}
