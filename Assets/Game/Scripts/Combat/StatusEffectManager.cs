using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageTaker))]
public class StatusEffectManager : MonoBehaviour
{
    private DamageTaker damageTaker;

    private List<StatusEffect> activeEffects = new List<StatusEffect>();


    private void Start()
    {
        damageTaker = GetComponent<DamageTaker>();

        foreach (var effect in activeEffects)
        {
            effect.StartEffect(damageTaker);
        }
    }

    void Update()
    {

        foreach (var effect in activeEffects)
        {
            if (effect.UpdateEffect(damageTaker))
            {
                effect.EndEffect(damageTaker);
                activeEffects.Remove(effect);
            }
        }
    }

    public void AddEffect(StatusEffect newEffect)
    {
        StatusEffect effectInstance = Instantiate(newEffect);
        effectInstance.Initialize();
        activeEffects.Add(effectInstance);
    }
}