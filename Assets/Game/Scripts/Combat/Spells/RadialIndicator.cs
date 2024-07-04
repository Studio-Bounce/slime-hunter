using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialIndicator : SpellIndicator
{
    public Color activeColor = Color.white;
    public Color inactiveColor = Color.red;

    public float castRange;
    public float radiusOfEffect;
    public LayerMask hitLayers;

    [SerializeField] private Transform player;
    [SerializeField] private Transform sourceIndicator;
    [SerializeField] private Transform targetIndicator;

    [SerializeField] private Renderer sourceRenderer;
    [SerializeField] private Renderer targetRenderer;

    public override Vector3 GetTarget {  get { return targetIndicator.transform.position; } }

    private void Start()
    {
        Debug.Assert(player != null, "Requires transform for player");
        Debug.Assert(sourceIndicator != null, "Requires prefab for sourceIndicator");
        Debug.Assert(targetIndicator != null, "Requires prefab for targetIndicator");

        sourceIndicator.localScale = new Vector3(castRange, castRange, castRange);
        targetIndicator.localScale = new Vector3(radiusOfEffect, radiusOfEffect, radiusOfEffect);

        Material material = sourceRenderer.material;
        float thickness = material.GetFloat("_Thickness");
        float feathering = material.GetFloat("_Feathering");
        material.SetFloat("_Thickness", thickness / castRange);
        material.SetFloat("_Feathering", feathering / castRange);
        sourceRenderer.material.SetColor("_Color", activeColor);


        material = targetRenderer.material;
        thickness = material.GetFloat("_Thickness");
        feathering = material.GetFloat("_Feathering");
        material.SetFloat("_Thickness", thickness / radiusOfEffect);
        material.SetFloat("_Feathering", feathering / radiusOfEffect);
        targetRenderer.material.SetColor("_Color", activeColor);
        HideIndicator();
    }

    private void Update()
    {
        sourceIndicator.transform.position = player.position;

        Ray ray = CameraManager.ActiveCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitLayers))
        {
            Vector3 direction = hit.point - player.position;

            if (direction.magnitude > castRange)
            {
                direction = direction.normalized * castRange;
            }

            targetIndicator.position = player.position + direction;
        }
    }

    public override void ShowIndicator()
    {
        gameObject.SetActive(true);
    }

    public override void HideIndicator()
    {
        gameObject.SetActive(false);
    }

    public override void SetReady(bool ready)
    {
        Color col = ready ? activeColor : inactiveColor;
        sourceRenderer.material.SetColor("_Color", col);
        targetRenderer.material.SetColor("_Color", col);
    }
}
