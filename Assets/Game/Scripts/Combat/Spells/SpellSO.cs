using System;
using UnityEngine;

public enum IndicatorType
{
    RADIAL
}

[CreateAssetMenu(menuName = "Spell")]
public class SpellSO : ItemSO
{
    [Header("Spell Properties")]
    public Damage damage;
    public float castRange;
    public float areaOfEffect;
    public float cooldown;
    public Spell spellPrefab;
    public IndicatorType spellIndicator;

    [NonSerialized] private bool ready = true;

    public bool Ready
    {
        get { return ready; }
        set { ready = value; }
    }
}