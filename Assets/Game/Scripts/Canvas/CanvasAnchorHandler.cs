using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnchorHandler : MonoBehaviour
{
    public List<CanvasElement> canvasElements = new List<CanvasElement>();

    // Caching instead of getting from CanvasManager.Instance to prevent creation of new instance OnDestroy
    CanvasManager canvasManager = null;

    private void Awake()
    {
        canvasManager = CanvasManager.Instance;
    }

    private void Update()
    {
        foreach (CanvasElement el in canvasElements)
        {
            el.rect.anchoredPosition = CameraManager.ActiveCamera.WorldToScreenPoint(el.anchor.position);
            el.rect.anchoredPosition += el.offset;
        }
    }

    public void RegisterCanvasElement(CanvasElement element)
    {
        canvasElements.Add(element);
    }

    private void OnDestroy()
    {
        if (canvasManager != null)
        {
            canvasManager.RemoveCanvasAnchor(this);
        }
    }
}
