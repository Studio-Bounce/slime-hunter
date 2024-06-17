using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct CanvasElement
{
    public Transform anchor;
    public RectTransform rect;
    public Vector2 offset;
}

public class CanvasManager : Singleton<CanvasManager>
{
    public Canvas screenCanvas;
    private List<CanvasAnchorHandler> canvasAnchorList = new List<CanvasAnchorHandler>();
    
    private void Awake()
    {
        // Create a canvas if one doesn't exist
        if (screenCanvas == null)
        {
            Debug.Log("Created screen canvas");
            GameObject canvasObject = new GameObject("ScreenCanvas");
            canvasObject.transform.SetParent(null, false);
            screenCanvas = canvasObject.AddComponent<Canvas>();
            screenCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();
        }
    }

    // Note offset is in screenspace
    public CanvasElement AddAnchoredElement(Transform anchor, RectTransform rect, Vector2 offset = default(Vector2))
    {
        // Set element's parent to the screen canvas
        rect.SetParent(screenCanvas.transform);
        rect.localPosition = Vector3.zero;

        // Add a component to the anchor to handle canvas elements
        CanvasAnchorHandler handler = anchor.gameObject.GetComponent<CanvasAnchorHandler>();
        if (handler == null)
        {
            handler = anchor.gameObject.AddComponent<CanvasAnchorHandler>();
        }

        CanvasElement canvasElement = new CanvasElement
        {
            anchor = anchor,
            rect = rect,
            offset = offset
        };

        handler.RegisterCanvasElement(canvasElement);
        canvasAnchorList.Add(handler);
        return canvasElement;
    }

    public void RemoveCanvasAnchor(CanvasAnchorHandler canvasAnchor)
    {
        canvasAnchorList.Remove(canvasAnchor);
    }

    //private void OnDestroy()
    //{
    //    canvasAnchorList.Clear();
    //    // Ensure that screenCanvas is destroyed
    //    if (screenCanvas != null)
    //    {
    //        DestroyImmediate(screenCanvas.gameObject);
    //        screenCanvas = null;
    //    }
    //}
}
