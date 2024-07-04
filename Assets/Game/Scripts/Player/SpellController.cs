using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpellController : MonoBehaviour
{
    public bool isCasting = false;

    public SpellSO[] spells = new SpellSO[3];

    private int currentSpellIndex = 0;
    [SerializeField] private SpellIndicator radialIndicator;
    private SpellIndicator currentIndicator;

    private Animator _animator;
    private readonly int castTriggerHash = Animator.StringToHash("Cast");

    public SpellSO CurrentSpell
    {
        get { return spells[currentSpellIndex]; }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        foreach (SpellSO spell in spells) spell.Ready = true;
    }

    private void Start()
    {
        Debug.Assert(radialIndicator != null);
        radialIndicator.HideIndicator();
    }

    public void StartCast(int spellIndex)
    {
        currentSpellIndex = spellIndex;

        switch (CurrentSpell.spellIndicator)
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

    IEnumerator StopCast()
    {
        yield return new WaitForSeconds(0.4f); // TODO: Hardcoded 0.4 seconds
        isCasting = false;
    }

    public void Cast(InputAction.CallbackContext context)
    {
        if (currentIndicator != null && currentIndicator.isActiveAndEnabled && CurrentSpell.Ready)
        {
            isCasting = true;
            StartCoroutine(StopCast());
            CurrentSpell.Ready = false;
            currentIndicator.HideIndicator();
            // Cast spell
            Spell spell = Instantiate(CurrentSpell.spellPrefab);
            spell.transform.position = transform.position;
            spell.Cast(currentIndicator.GetTarget);
            StartCoroutine(StartCooldown(currentSpellIndex));
            // Rotate player to cast direction
            Vector3 castDirection = currentIndicator.GetTarget - transform.position;
            transform.forward = castDirection;
            _animator.SetTrigger(castTriggerHash);
        }
    }

    IEnumerator StartCooldown(int spellIndex)
    {
        HUDMenu hud = UIManager.Instance.HUDMenu as HUDMenu;

        float remainingCD = spells[spellIndex].cooldown;
        while (remainingCD > 0)
        {
            if (spellIndex == currentSpellIndex) currentIndicator.SetReady(false);
            remainingCD -= Time.deltaTime;
            hud.UpdateSpellCooldown(0, Mathf.CeilToInt(remainingCD));
            yield return null;
        }
        hud.UpdateSpellCooldown(0, 0);
        spells[spellIndex].Ready = true;
        if (spellIndex == currentSpellIndex) currentIndicator.SetReady(true);

    }
}