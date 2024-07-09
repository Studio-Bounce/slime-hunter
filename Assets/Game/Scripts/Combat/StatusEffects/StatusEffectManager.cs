using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusEffectManager : MonoBehaviour
{
    public StatusBar statusBar;
    private DynamicDamageTaker damageTaker;
    public List<StatusEffect> activeEffects = new List<StatusEffect>();
    public List<StatusEffect> effectsToRemove = new List<StatusEffect>();

    private void Start()
    {
        damageTaker = GetComponent<DynamicDamageTaker>();
        foreach (var effect in activeEffects)
        {
            effect.StartEffect(damageTaker);

            if (statusBar != null)
            {
                statusBar.AddStatusEffect(effect);
            }
        }
    }

    void Update()
    {
        foreach (var effect in activeEffects)
        {
            if (effect.UpdateEffect(damageTaker))
            {
                effect.EndEffect(damageTaker);
                effectsToRemove.Add(effect);
                statusBar.RemoveStatusEffect(effect);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            activeEffects.Remove(effect);
        }
        effectsToRemove.Clear();

        if (!damageTaker.isAlive)
        {
            statusBar.gameObject.SetActive(false);
        }
    }

    public void AddEffect(StatusEffect newEffect)
    {
        StatusEffect effectInstance = Instantiate(newEffect);
        effectInstance.Initialize();
        effectInstance.StartEffect(damageTaker);
        activeEffects.Add(effectInstance);

        statusBar.AddStatusEffect(effectInstance);
    }
}