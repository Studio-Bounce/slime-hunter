using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : Singleton<CanvasManager>
{
    public Canvas screenCanvas;

    private Dictionary<Transform, RectTransform> anchorToElementMap = new Dictionary<Transform, RectTransform>();

    private void Awake()
    {
        // Create a canvas if one doesn't exist
        if (screenCanvas == null)
        {
            GameObject canvasObject = new GameObject("ScreenCanvas");
            canvasObject.transform.SetParent(null, false);
            screenCanvas = canvasObject.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
        }
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the positions of elements to match anchor
        foreach (KeyValuePair<Transform, RectTransform> entry in anchorToElementMap)
        {
            Transform anchor = entry.Key;
            RectTransform element = entry.Value;

            element.anchoredPosition = Camera.main.WorldToScreenPoint(anchor.position);
        }
    }

    public void AddAnchoredElement(Transform anchor, RectTransform element)
    {
        element.SetParent(screenCanvas.transform);
    }
}
