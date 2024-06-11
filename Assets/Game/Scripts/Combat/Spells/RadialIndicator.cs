using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialIndicator : SpellIndicator
{
    public GameObject indicatorPrefab;

    public float castRange;
    public float radiusOfEffect;

    private Transform sourceIndicator;
    private Transform targetIndicator;

    private void Start()
    {
        Debug.Assert(indicatorPrefab != null, "Requires prefab for indicator");
        sourceIndicator = Instantiate(indicatorPrefab, transform, false).transform;
        sourceIndicator.localScale = new Vector3(castRange, castRange, castRange);
        sourceIndicator.gameObject.SetActive(false);

        targetIndicator = Instantiate(indicatorPrefab, transform, false).transform;
        targetIndicator.localScale = new Vector3(radiusOfEffect, radiusOfEffect, radiusOfEffect);
        targetIndicator.gameObject.SetActive(false);
    }

    public override void ShowIndicator()
    {
        sourceIndicator?.gameObject.SetActive(true);
        targetIndicator?.gameObject.SetActive(true);
    }

    public override void HideIndicator()
    {
        sourceIndicator?.gameObject.SetActive(false);
        targetIndicator?.gameObject.SetActive(false);
    }
}
