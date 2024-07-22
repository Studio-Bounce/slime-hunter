using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable]
public class HealSpell : Spell
{
    public VisualEffect effect;

    private int healAmount = 0;

    private void Awake()
    {
        Debug.Assert(effect != null, "Requires a VisualEffect");
    }

    public override void Cast(Vector3 target = default)
    {
        StartCoroutine(RunCast());
    }

    IEnumerator RunCast()
    {
        effect.Play();
        GameManager.Instance.PlayerHealth += healAmount;
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    public override void Initialize(SpellSO spellSO)
    {
        healAmount = (int)spellSO.damage.value;
    }
}
