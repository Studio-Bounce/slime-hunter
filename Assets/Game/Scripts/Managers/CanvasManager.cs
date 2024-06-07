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

    // Update is called once per frame
    private void Update()
    {
        // Update the positions of elements to match anchor
        foreach (KeyValuePair<Transform, RectTransform> entry in anchorToElementMap)
        {
            Transform anchor = entry.Key;
            RectTransform element = entry.Value;
            element.anchoredPosition = CameraManager.Instance.ActiveCamera.WorldToScreenPoint(anchor.position);
        }
    }

    public void AddAnchoredElement(Transform anchor, RectTransform element)
    {
        // Set element's parent to the screen canvas
        element.SetParent(screenCanvas.transform);

        // Add a component to the anchor to handle OnDestroy event
        AnchorDestroyHandler handler = anchor.gameObject.GetComponent<AnchorDestroyHandler>();
        if (handler == null)
        {
            anchor.gameObject.AddComponent<AnchorDestroyHandler>();
        }

        // Add to the map
        anchorToElementMap[anchor] = element;
    }

    public void RemoveAnchoredElement(Transform anchor)
    {
        if (anchorToElementMap.ContainsKey(anchor))
        {
            RectTransform element = anchorToElementMap[anchor];
            anchorToElementMap.Remove(anchor);
            Destroy(element.gameObject);
        }
    }
}
