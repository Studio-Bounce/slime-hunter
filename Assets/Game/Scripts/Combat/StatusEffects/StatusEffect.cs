using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    public StatusEffectIcon icon;
    public string effectName;
    public float duration = 1;
    public float tickInterval = 1;
    private float tickTimer = 0;

    private float timeRemaining;
    

    public void Initialize()
    {
        timeRemaining = duration;
    }

    public void StartEffect(DynamicDamageTaker taker)
    {
        OnStartEffect(taker);
    }

    public void Reset()
    {
        timeRemaining = duration;
    }

    public bool UpdateEffect(DynamicDamageTaker taker)
    {
        timeRemaining -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            OnUpdateEffect(taker);
            tickTimer = 0f;
        }

        return timeRemaining <= 0f;
    }

    public void EndEffect(DynamicDamageTaker taker)
    {
        OnEndEffect(taker);
    }

    protected abstract void OnStartEffect(DynamicDamageTaker taker);

    protected abstract void OnUpdateEffect(DynamicDamageTaker taker);

    protected abstract void OnEndEffect(DynamicDamageTaker taker);

}
