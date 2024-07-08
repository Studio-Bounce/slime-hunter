using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class VFXEnforceTimeScale : MonoBehaviour
{
    private VisualEffect effect;

    private void Start()
    {
        effect = GetComponent<VisualEffect>();
    }

    void Update()
    {

    }
}
