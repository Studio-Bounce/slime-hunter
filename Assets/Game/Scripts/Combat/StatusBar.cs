using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class StatusBar : MonoBehaviour
{
    public Vector2 iconSize = new Vector2(50, 50);

    private Transform ownerTransform;
    private RectTransform statusBarRoot;
    private Dictionary<StatusEffect, Image> statusToImageMap = new Dictionary<StatusEffect, Image>();

    // Start is called before the first frame update
    void Start()
    {
        statusBarRoot = transform.GetComponent<RectTransform>();
    }

    public void Initialize(Transform trans)
    {
        ownerTransform = trans;
        transform.SetParent(GameManager.Instance.screenCanvas.transform);
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (!statusToImageMap.ContainsKey(effect))
        {
            GameObject imageObject = new GameObject("StatusEffectImage");
            imageObject.transform.SetParent(statusBarRoot.transform, false);
            Image imageComponent = imageObject.AddComponent<Image>();
            imageComponent.sprite = effect.icon;
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

    // Update is called once per frame
    void Update()
    {
        if (ownerTransform != null)
        {
            statusBarRoot.anchoredPosition = Camera.main.WorldToScreenPoint(ownerTransform.position);
        } else
        {
            Destroy(this);
        }
    }
}
