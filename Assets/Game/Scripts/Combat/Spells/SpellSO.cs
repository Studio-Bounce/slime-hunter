using UnityEngine;

public enum IndicatorType
{
    RADIAL
}

[CreateAssetMenu(menuName = "Spell")]
public class SpellSO : ScriptableObject
{
    public string spellName;
    [TextArea] public string description;
    public Sprite icon;
    public Damage damage;
    public float cooldown;
    public float castTime;
    public float manaCost;
    public Spell spellPrefab;
    public IndicatorType spellIndicator;
}