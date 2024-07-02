using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellController : MonoBehaviour
{
    public SpellSO[] spells = new SpellSO[3];
    [SerializeField] private SpellIndicator radialIndicator;
    private SpellIndicator currentIndicator;

    public void Start()
    {
        Debug.Assert(radialIndicator != null);
        radialIndicator.HideIndicator();
    }

    public void StartCast(int spellIndex)
    {
        switch (spells[spellIndex].spellIndicator)
        {
            case IndicatorType.RADIAL:
                currentIndicator = radialIndicator;
                break;
        }

        if (!currentIndicator.isActiveAndEnabled)
        {
            currentIndicator.ShowIndicator();
        } else
        {
            currentIndicator.HideIndicator();
        }
    }

    public void Cast(InputAction.CallbackContext context)
    {
        if (currentIndicator != null && currentIndicator.isActiveAndEnabled)
        {
            // Hard code to use full stamina
            if (!GameManager.Instance.IsFullStamina) return;
            GameManager.Instance.UseStamina(GameManager.Instance.PlayerStamina);

            Spell spell = Instantiate(spells[0].spellPrefab);
            spell.transform.position = transform.position;
            spell.Cast(currentIndicator.GetTarget);

            currentIndicator.HideIndicator();
        }
    }
}