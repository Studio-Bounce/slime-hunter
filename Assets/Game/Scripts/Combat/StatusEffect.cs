using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    public string effectName;
    public float duration = 1;
    public float tickInterval = 1;

    private float timeRemaining;
    private float tickTime = 0;

    public void Initialize()
    {
        timeRemaining = duration;
    }

    public void StartEffect(DamageTaker taker)
    {
        OnStartEffect(taker);
    }

    public bool UpdateEffect(DamageTaker taker)
    {
        OnStartEffect(taker);
        timeRemaining -= Time.deltaTime;
        tickTime += Time.deltaTime;

        if (tickTime >= tickInterval)
        {
            OnUpdateEffect(taker);
            tickTime = 0f;
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
