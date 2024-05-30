using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    public string effectName;
    public float duration = 1;
    public float tickInterval = 1;
    private float tickTimer = 0;

    private float timeRemaining;
    

    public void Initialize()
    {
        timeRemaining = duration;
    }

    public void StartEffect(DamageTaker taker)
    {
        OnStartEffect(taker);
    }

    public void Reset()
    {
        timeRemaining = duration;
    }

    public bool UpdateEffect(DamageTaker taker)
    {
        OnStartEffect(taker);
        timeRemaining -= Time.deltaTime;
        tickTimer += Time.deltaTime;

        if (tickTimer >= tickInterval)
        {
            OnUpdateEffect(taker);
            tickTimer = 0f;
        }

        return timeRemaining <= 0f;
    }

    public void EndEffect(DamageTaker taker)
    {
        OnEndEffect(taker);
    }

    protected abstract void OnStartEffect(DamageTaker taker);

    protected abstract void OnUpdateEffect(DamageTaker taker);

    protected abstract void OnEndEffect(DamageTaker taker);

}
