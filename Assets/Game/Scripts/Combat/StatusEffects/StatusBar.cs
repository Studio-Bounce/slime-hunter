using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class StatusBar : MonoBehaviour
{
    private RectTransform statusBarRoot;
    private Dictionary<StatusEffect, StatusEffectIcon> statusToIconMap = new Dictionary<StatusEffect, StatusEffectIcon>();

    void Awake()
    {
        statusBarRoot = transform.GetComponent<RectTransform>();
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        if (!statusToIconMap.ContainsKey(effect))
        {
            StatusEffectIcon icon = Instantiate(effect.icon, statusBarRoot.transform);
            statusToIconMap[effect] = icon;
        }
    }

    public void RemoveStatusEffect(StatusEffect effect)
    {
        if (statusToIconMap.ContainsKey(effect))
        {
            Destroy(statusToIconMap[effect].gameObject);
            statusToIconMap.Remove(effect);
        }
    }
}
