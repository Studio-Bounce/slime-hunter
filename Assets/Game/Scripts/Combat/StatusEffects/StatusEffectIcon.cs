using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Image), typeof(RectTransform))]
[System.Serializable]
public class StatusEffectIcon : MonoBehaviour
{
    private RectTransform rect;
    private Image image;
    void Start()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        Debug.Assert(image != null);
        Debug.Assert(rect != null);
    }
}
