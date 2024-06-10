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

    private void Start()
    {
        CanvasManager.Instance.AddAnchoredElement(transform.parent, statusBarRoot, new Vector2(0, 100));
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (!statusToImageMap.ContainsKey(effect))
        {
            GameObject imageObject = new GameObject("StatusEffectImage", typeof(Image));
            imageObject.transform.SetParent(statusBarRoot.transform, false);
            Image imageComponent = imageObject.GetComponent<Image>();
            imageComponent.rectTransform.sizeDelta = iconSize;
            imageComponent.sprite = effect.icon;
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
