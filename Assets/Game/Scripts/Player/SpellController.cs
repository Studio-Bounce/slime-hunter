using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class SpellController : MonoBehaviour
{
    [NonSerialized] public SpellSO[] availableSpells = new SpellSO[2];
    [NonSerialized] public bool isCasting = false;

    private int currentSpellIndex = 0;
    private int lastSpellIndex = -1;

    // Indicator & UI
    [SerializeField] private SpellIndicator radialIndicator;
    private SpellIndicator currentIndicator;
    private HUDMenu hudMenu;

    // Animations
    private Animator _animator;
    private readonly int castTriggerHash = Animator.StringToHash("Cast");

    public SpellSO CurrentSpell
    {
        get { return availableSpells[currentSpellIndex]; }
    }

    private void Awake()
    {
        _animator = GetComponent<PlayerController>()?.animator;
        Debug.Assert(_animator != null);
        hudMenu = UIManager.Instance.HUDMenu as HUDMenu;
    }

    private void Start()
    {
        Debug.Assert(radialIndicator != null);
        radialIndicator.HideIndicator();
        LoadSpellIcons();
    }

    public void LoadSpellIcons()
    {
        if (hudMenu == null)
            return;

        for (int i = 0; i < availableSpells.Length; i++)
        {
            hudMenu?.SetSpellIcon(i + 1, availableSpells[i]?.icon);
        }
        UpdateSpellUI();
    }

    public void StartCast(int spellIndex)
    {
        // Set indicator type based on spell
        lastSpellIndex = currentSpellIndex;
        currentSpellIndex = spellIndex;
        if (CurrentSpell == null)
        {
            Debug.Log("No spell equipped in slot");
            return;
        }

        switch (CurrentSpell.spellIndicator)
        {
            case IndicatorType.RADIAL:
                currentIndicator = radialIndicator;
                break;
        }

        ToggleIndicator();
    }

    private void ToggleIndicator()
    {
        if (lastSpellIndex != currentSpellIndex || !currentIndicator.Active)
        {
            currentIndicator.ShowIndicator(CurrentSpell);
            currentIndicator.ToggleReady(CurrentSpell.Ready);
            UpdateSpellUI();
        }
        else
        {
            currentIndicator.HideIndicator();

        }
    }

    private void UpdateSpellUI()
    {
        hudMenu?.SetSpellActive(currentSpellIndex+1);
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
            Spell spell = Instantiate(CurrentSpell.spellPrefab, transform.position, Quaternion.identity);
            spell.Initialize(CurrentSpell);
            spell.Cast(currentIndicator.GetTarget);
            StartCoroutine(StartCooldown(currentSpellIndex));
            // Rotate player to cast direction and animate
            Vector3 castDirection = currentIndicator.GetTarget - transform.position;
            transform.forward = castDirection;
            _animator.SetTrigger(castTriggerHash);
        }
    }

    IEnumerator StartCooldown(int spellIndex)
    {
        StartCoroutine(StopCast());
        float remainingCD = availableSpells[spellIndex].cooldown;

        if (spellIndex == currentSpellIndex) currentIndicator.ToggleReady(false);
        while (remainingCD > 0)
        {
            remainingCD -= Time.deltaTime;
            hudMenu?.UpdateSpellCooldown(spellIndex+1, Mathf.CeilToInt(remainingCD));
            yield return null;
        }

        hudMenu?.UpdateSpellCooldown(spellIndex + 1, 0);
        availableSpells[spellIndex].Ready = true;
        if (spellIndex == currentSpellIndex) currentIndicator.ToggleReady(true);

    }
}