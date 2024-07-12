using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDMenu : Menu
{
    [Range(1, 100)][SerializeField] int maxAlert = 50;
    [SerializeField] float damageAlertTime = 1.0f;

    VisualElement root;

    // Player
    VisualElement healthDamageVE;
    ProgressBar healthProgressBar;
    ProgressBar staminaProgressBar;

    // Spells
    readonly string spellDisabledStyle = "spell-glyph-disabled";
    readonly string spellActiveStyle = "spell-glyph-active";

    // Quests
    VisualElement questNameVE;
    Label questNameLabel;
    Label questDescriptionLabel;

    bool redAlertUp = false;

    // Cached as they're getting called in OnDestroy
    GameManager gameManager;
    QuestManager questManager;

    void Start()
    {
        root = uiDocument.rootVisualElement;
        gameManager = GameManager.Instance;
        questManager = QuestManager.Instance;

        // Player
        healthDamageVE = root.Q<VisualElement>("Red_Alert");
        VisualElement background = root.Q<VisualElement>("Background");
        VisualElement leftBg = background.Q<VisualElement>("Left");
        VisualElement leftStatusArea = leftBg.Q<VisualElement>("StatusArea");
        VisualElement statusBars = leftStatusArea.Q<VisualElement>("Bars");

        VisualElement healthVE = statusBars.Q<VisualElement>("Health");
        healthProgressBar = healthVE.Q<ProgressBar>("HealthBar");
        gameManager.OnPlayerHealthChange += UpdateHealthBar;
        UpdateHealthBar(gameManager.PlayerHealth);

        VisualElement staminaVE = statusBars.Q<VisualElement>("Stamina");
        staminaProgressBar = staminaVE.Q<ProgressBar>("StaminaBar");
        gameManager.OnPlayerStaminaChange += UpdateStaminaBar;
        UpdateStaminaBar(gameManager.PlayerStamina);

        // Spells

        // Quests
        VisualElement questContainer = root.Q<VisualElement>("QuestContainer");
        questNameVE = questContainer.Q<VisualElement>("Header");
        questNameLabel = questNameVE.Q<Label>("Quest-Name");
        questDescriptionLabel = questContainer.Q<Label>("Content");
        questManager.OnActiveQuestChange += UpdateActiveQuest;
        
        questNameVE.style.display = DisplayStyle.None;
        questDescriptionLabel.style.display = DisplayStyle.None;
    }

    // ------------------------------ Health ------------------------------

    void UpdateHealthBar(int health)
    {
        // Damage red alert UI
        if (!redAlertUp)
        {
            StartCoroutine(InitiateRedAlert());
        }

        if (healthProgressBar != null)
        {
            healthProgressBar.value = (float)health / GameManager.Instance.PlayerMaxHealth;
        }
    }

    IEnumerator InitiateRedAlert()
    {
        redAlertUp = true;
        // Set red alert to maxAlert. Reduce it to 0 over time
        int opacity = maxAlert;
        healthDamageVE.style.opacity = opacity;
        float timeDelta = damageAlertTime / maxAlert;

        while (opacity > 0)
        {
            --opacity;
            healthDamageVE.style.opacity = opacity;

            yield return new WaitForSeconds(timeDelta);
        }
        redAlertUp = false;
    }

    // ------------------------------ Stamina ------------------------------

    void UpdateStaminaBar(int stamina)
    {
        if (staminaProgressBar != null)
        {
            staminaProgressBar.value = (float)stamina / GameManager.Instance.PlayerMaxStamina;
        }
    }

    // ------------------------------ Spells -------------------------------

    public void UpdateSpellCooldown(int spellNumber, int value)
    {
        VisualElement skillElement = root.Q<VisualElement>($"Spell{spellNumber}");
        Label skillTimer = skillElement.Q<Label>("Timer");

        if (value > 0) {
            skillElement.AddToClassList(spellDisabledStyle);
        } else
        {
            skillElement.RemoveFromClassList(spellDisabledStyle);
        }

        skillTimer.text = value.ToString();
    }

    public void SetSpellActive(int spellNumber)
    {
        // Set active style to selected spell and remove from rest
        var skillElements = root.Query<VisualElement>().Where(
            ve => ve.name != null && ve.name.StartsWith("Spell")
            ).ToList();
        foreach (var el in skillElements)
        {
            if (el.name.Contains(spellNumber.ToString()))
            {
                el.AddToClassList(spellActiveStyle);
            } else
            {
                el.RemoveFromClassList(spellActiveStyle);
            }
        }
    }

    public void SetSpellIcon(int spellNumber, Sprite icon)
    {
        VisualElement skillElement = root.Q<VisualElement>($"Spell{spellNumber}");
        skillElement.style.backgroundImage = icon?.texture;
    }

    // ------------------------------ Quests -------------------------------

    void UpdateActiveQuest(string questName, string questDescription)
    {
        if (questName == "")
        {
            questNameVE.style.display = DisplayStyle.None;
            questDescriptionLabel.style.display = DisplayStyle.None;
            return;
        }
        questNameVE.style.display = DisplayStyle.Flex;
        questDescriptionLabel.style.display = DisplayStyle.Flex;
        questNameLabel.text = questName;
        questDescriptionLabel.text = questDescription;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerHealthChange -= UpdateHealthBar;
            gameManager.OnPlayerStaminaChange -= UpdateStaminaBar;
        }
        if (questManager != null)
        {
            questManager.OnActiveQuestChange -= UpdateActiveQuest;
        }
    }
}
