using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDMenu : Menu
{
    [Range(1, 100)][SerializeField] int maxAlert = 50;
    [SerializeField] float damageAlertTime = 1.0f;

    VisualElement healthDamageVE;
    ProgressBar healthProgressBar;
    ProgressBar staminaProgressBar;

    bool redAlertUp = false;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;

        healthDamageVE = root.Q<VisualElement>("Red_Alert");
        VisualElement background = root.Q<VisualElement>("Background");
        VisualElement leftBg = background.Q<VisualElement>("Left");
        VisualElement leftStatusArea = leftBg.Q<VisualElement>("StatusArea");
        VisualElement statusBars = leftStatusArea.Q<VisualElement>("Bars");

        VisualElement healthVE = statusBars.Q<VisualElement>("Health");
        healthProgressBar = healthVE.Q<ProgressBar>("HealthBar");
        GameManager.Instance.OnPlayerHealthChange += UpdateHealthBar;
        UpdateHealthBar(GameManager.Instance.PlayerHealth);

        VisualElement staminaVE = statusBars.Q<VisualElement>("Stamina");
        staminaProgressBar = staminaVE.Q<ProgressBar>("StaminaBar");
        GameManager.Instance.OnPlayerStaminaChange += UpdateStaminaBar;
        UpdateStaminaBar(GameManager.Instance.PlayerStamina);
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
}
