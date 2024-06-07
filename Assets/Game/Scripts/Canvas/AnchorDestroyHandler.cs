using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorDestroyHandler : MonoBehaviour
{
    private void OnDestroy()
    {
        if (CanvasManager.Instance)
        {
            CanvasManager.Instance.RemoveAnchoredElement(transform);
        }
    }
}
