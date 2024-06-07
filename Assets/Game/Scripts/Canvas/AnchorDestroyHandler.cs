using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorDestroyHandler : MonoBehaviour
{
    private void OnDestroy()
    {
        CanvasManager.Instance.RemoveAnchoredElement(transform);
    }
}
