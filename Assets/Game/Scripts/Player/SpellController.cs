using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpellController : MonoBehaviour
{
    public bool isCasting = false;

    public SpellSO[] spells = new SpellSO[2];

    private int currentSpellIndex = 0;
    private int lastSpellIndex = -1;

    // Indicator & UI
    [SerializeField] private SpellIndicator radialIndicator;
    private SpellIndicator currentIndicator;
    HUDMenu hudMenu;


    // Animations
    private Animator _animator;
    private readonly int castTriggerHash = Animator.StringToHash("Cast");

    public SpellSO CurrentSpell
    {
        get { return spells[currentSpellIndex]; }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        hudMenu = UIManager.Instance.HUDMenu as HUDMenu;
        foreach (SpellSO spell in spells) spell.Ready = true;
    }

    private void Start()
    {
        Debug.Assert(radialIndicator != null);
        radialIndicator.HideIndicator();
    }

    public void StartCast(int spellIndex)
    {
        // Set indicator type based on spell
        lastSpellIndex = currentSpellIndex;
        currentSpellIndex = spellIndex;
        switch (CurrentSpell.spellIndicator)
        {
            case IndicatorType.RADIAL:
                currentIndicator = radialIndicator;
                break;
        }

        // Toggle indicator
        if (!currentIndicator.isActiveAndEnabled)
        {
            currentIndicator.SetReady(spells[spellIndex].Ready);
            hudMenu.SetSpellActive(currentSpellIndex+1);
            currentIndicator.ShowIndicator();
        } else if (lastSpellIndex == currentSpellIndex)
        {
            currentIndicator.HideIndicator();
        } else
        {
            // If switching to different spell while previous indicator is active
            currentIndicator.SetReady(spells[spellIndex].Ready);
            hudMenu.SetSpellActive(currentSpellIndex + 1);
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
        StartCoroutine(StopCast());
        float remainingCD = spells[spellIndex].cooldown;

        if (spellIndex == currentSpellIndex) currentIndicator.SetReady(false);
        while (remainingCD > 0)
        {
            remainingCD -= Time.deltaTime;
            hudMenu.UpdateSpellCooldown(spellIndex+1, Mathf.CeilToInt(remainingCD));
            yield return null;
        }

        hudMenu.UpdateSpellCooldown(spellIndex + 1, 0);
        spells[spellIndex].Ready = true;
        if (spellIndex == currentSpellIndex) currentIndicator.SetReady(true);

    }
}