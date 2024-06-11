using UnityEngine;

public enum IndicatorType
{
    RADIAL
}

public abstract class SpellSO : ScriptableObject
{
    public string spellName;
    [TextArea] public string description;
    public Sprite icon;
    public Damage damage;
    public float cooldown;
    public float castTime;
    public float manaCost;
    public GameObject spellEffectPrefab;
    public IndicatorType spellIndicator;

    public abstract void Cast();
}