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
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (activeEffects[i].UpdateEffect(damageTaker))
            {
                activeEffects[i].EndEffect(damageTaker);
                activeEffects.RemoveAt(i);
                
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