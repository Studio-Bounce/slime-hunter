using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageTaker))]
public class StatusEffectManager : MonoBehaviour
{
    private DamageTaker damageTaker;

    public GameObject statusBarPrefab;
    private StatusBar statusBar;
    public bool showStatusBar = true;
    public List<StatusEffect> activeEffects = new List<StatusEffect>();
    public List<StatusEffect> effectsToRemove = new List<StatusEffect>();



    private void Start()
    {
        damageTaker = GetComponent<DamageTaker>();
        statusBar = GetComponent<StatusBar>();

        InitializeStatusBar();

        foreach (var effect in activeEffects)
        {
            effect.StartEffect(damageTaker);

            if (statusBar != null)
            {
                statusBar.AddStatusEffect(effect);
            }
        }
    }

    private void InitializeStatusBar()
    {
        if (showStatusBar)
        {
            statusBar = Instantiate(statusBarPrefab)?.GetComponent<StatusBar>();
            statusBar.Initialize(transform);
        }
    }

    void Update()
    {
        foreach (var effect in activeEffects)
        {
            if (effect.UpdateEffect(damageTaker))
            {
                effect.EndEffect(damageTaker);
                statusBar.RemoveStatusEffect(effect);
                effectsToRemove.Add(effect);
            }
        }

        foreach (var effect in effectsToRemove)
        {
            activeEffects.Remove(effect);
        }
        effectsToRemove.Clear();
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