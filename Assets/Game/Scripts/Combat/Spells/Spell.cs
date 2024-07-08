using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Spell : MonoBehaviour // Ideally this would be an interface but I need it to be serializable
{
    public abstract void Initialize(SpellSO spellSO);
    public abstract void Cast(Vector3 target = default(Vector3));
}
