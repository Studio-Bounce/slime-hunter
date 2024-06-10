using UnityEngine;

[CreateAssetMenu(fileName = "NewSpell", menuName = "Spells/Spell")]
public abstract class Spell : ScriptableObject
{
    public string spellName;
    public Sprite icon;
    public Damage damage;
    public float cooldown;
    public float castTime;
    public float manaCost;
    public GameObject spellEffectPrefab;

    [TextArea]
    public string description;

    public abstract void Cast();
}