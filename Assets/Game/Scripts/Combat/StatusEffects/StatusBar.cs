using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class StatusBar : MonoBehaviour
{
    public Vector2 iconSize = new Vector2(50, 50);

    private RectTransform statusBarRoot;
    private Dictionary<StatusEffect, Image> statusToImageMap = new Dictionary<StatusEffect, Image>();

    void Awake()
    {
        statusBarRoot = transform.GetComponent<RectTransform>();
    }

    public void Initialize(Transform trans)
    {
        CanvasManager.Instance.AddAnchoredElement(trans, statusBarRoot);
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (!statusToImageMap.ContainsKey(effect))
        {
            GameObject imageObject = new GameObject("StatusEffectImage");
            imageObject.transform.SetParent(statusBarRoot.transform, false);
            imageObject.transform.localPosition = Vector3.zero;
            imageObject.transform.localScale = Vector3.one;
            Image imageComponent = imageObject.AddComponent<Image>();
            imageComponent.sprite = effect.icon;
            imageComponent.rectTransform.localScale = Vector3.one;
            imageComponent.rectTransform.sizeDelta = iconSize;
            statusToImageMap[effect] = imageComponent;
        }
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        if (statusToImageMap.ContainsKey(effect))
        {
            Destroy(statusToImageMap[effect].gameObject);
            statusToImageMap.Remove(effect);
        }
    }
}
