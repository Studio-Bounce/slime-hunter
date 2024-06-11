using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpellController : MonoBehaviour
{
    public SpellSO[] spells = new SpellSO[3];

    public SpellIndicator radialIndicator;
    private SpellIndicator currentIndicator;

    public void Start()
    {
        Debug.Assert(radialIndicator != null);
        radialIndicator.HideIndicator();
    }

    public void StartCast(InputAction.CallbackContext context)
    {
        switch (spells[0].spellIndicator)
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
            spells[0].Cast();
            currentIndicator.HideIndicator();
        }
    }
}
