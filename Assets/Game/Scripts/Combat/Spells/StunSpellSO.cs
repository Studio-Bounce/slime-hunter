using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/StunSpell")]
public class StunSpellSO : SpellSO
{
    public override void Cast()
    {
        Debug.Log("Cast");
    }
}
