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
    [SerializeField] private SpellIndicator radialRangedIndicator;
    private SpellIndicator currentIndicator;
    private HUDMenu hudMenu;

    // Animations
    private Animator _animator;
    private readonly int castTriggerHash = Animator.StringToHash("Cast");

    // Inventory
    private InventoryManager inventoryManager;

    public SpellSO CurrentSpell
    {
        get { return availableSpells[currentSpellIndex]; }
    }

    private void Awake()
    {
        _animator = GetComponent<PlayerController>()?.animator;
        Debug.Assert(_animator != null);
        inventoryManager = InventoryManager.Instance;
        inventoryManager.OnEquippedSpellsChanged += OnSpellUpdate;
        hudMenu = UIManager.Instance.HUDMenu as HUDMenu;
    }

    private void Start()
    {
        Debug.Assert(radialRangedIndicator != null);
        radialRangedIndicator.HideIndicator();
        ChangeSpell(currentSpellIndex);
        LoadSpellIcons();
    }

    private void OnDestroy()
    {
        inventoryManager.OnEquippedSpellsChanged -= OnSpellUpdate;
    }

    private void OnSpellUpdate(SpellSO[] spells)
    {
        availableSpells = spells;
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

    public void ChangeSpell(int spellIndex)
    {
        if (availableSpells[spellIndex] == null)
        {
            Debug.Log("No spell equipped in slot");
            return;
        }
        lastSpellIndex = currentSpellIndex;
        currentSpellIndex = spellIndex;
        UpdateSpellUI();
    }


    private void ToggleIndicator()
    {
        switch (CurrentSpell.spellIndicator)
        {
            case IndicatorType.RADIAL:
                currentIndicator = radialRangedIndicator;
                break;
        }

        if (lastSpellIndex != currentSpellIndex || !currentIndicator.Active)
        {
            currentIndicator.ShowIndicator(CurrentSpell);
            currentIndicator.ToggleReady(CurrentSpell.Ready);
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
        yield return new WaitForSeconds(0.4f); // TODO: Hardcoded 0.4 seconds animation
        isCasting = false;
    }

    public void CancelSpell(InputAction.CallbackContext context)
    {
        isCasting = false;
        if (currentIndicator != null) currentIndicator.HideIndicator();
    }

    public void AimSpell(InputAction.CallbackContext context)
    {
        if (CurrentSpell == null) return;
        ToggleIndicator();
    }

    public void CastSpell(InputAction.CallbackContext context)
    {
        if (CurrentSpell == null) return;
        if (currentIndicator != null && currentIndicator.Active && CurrentSpell.Ready)
        {
            isCasting = true;
            CurrentSpell.Ready = false;
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
        currentIndicator.HideIndicator();
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