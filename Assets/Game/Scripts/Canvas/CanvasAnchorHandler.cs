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
            Vector3 elScreenPosition = CameraManager.ActiveCamera.WorldToScreenPoint(el.anchor.position);
            bool isOffScreen = Utils.IsVector3OffScreen(elScreenPosition);

            // Save some canvas re-renders by not changing position if element is offscreen
            // If currently element is on-screen but it should be off-screen, move it off-screen
            if (!isOffScreen || !(Utils.IsVector3OffScreen(el.rect.anchoredPosition)))
            {
                el.rect.anchoredPosition = elScreenPosition;
                el.rect.anchoredPosition += el.offset;
            }
        }
    }

    public void RegisterCanvasElement(CanvasElement element)
    {
        canvasElements.Add(element);
    }

    private void OnDestroy()
    {
        foreach (CanvasElement el in canvasElements)
        {
            if (el.rect != null)
            {
                Destroy(el.rect.gameObject);
            }
        }
        if (canvasManager != null)
        {
            canvasManager.RemoveCanvasAnchor(this);
        }
    }
}
