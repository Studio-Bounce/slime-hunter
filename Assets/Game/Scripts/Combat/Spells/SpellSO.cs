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
    public Spell spellPrefab;
    public IndicatorType spellIndicator;

    private bool ready = true;

    public bool Ready
    {
        get { return ready; }
        set { ready = value; }
    }
}