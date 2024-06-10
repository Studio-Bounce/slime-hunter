using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NotificationType
{
    FADE
}

[RequireComponent(typeof(RectTransform), typeof(CanvasRenderer))]
public class Notification : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasRenderer canvasRenderer;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasRenderer = GetComponent<CanvasRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
