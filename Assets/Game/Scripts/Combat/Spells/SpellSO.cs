using UnityEngine;

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
    public SpellIndicator spellIndicator;

    public abstract void Cast();
}