using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnchorHandler : MonoBehaviour
{
    public List<CanvasElement> canvasElements = new List<CanvasElement>();

    private void Update()
    {
        foreach (CanvasElement el in canvasElements)
        {
            el.rect.anchoredPosition = CameraManager.Instance.ActiveCamera.WorldToScreenPoint(el.anchor.position);
        }
    }

    public void RegisterCanvasElement(CanvasElement element)
    {
        canvasElements.Add(element);
    }

    private void OnDestroy()
    {
        if (CanvasManager.Instance)
        {
            CanvasManager.Instance.RemoveCanvasAnchor(this);
        }
    }
}
