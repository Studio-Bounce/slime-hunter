using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDMenu : Menu
{
    [Range(1, 100)][SerializeField] int maxAlert = 50;
    [SerializeField] float damageAlertTime = 1.0f;
    ProgressBar healthProgressBar;
    VisualElement healthDamageVE;

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
    }

    void UpdateHealthBar(int health)
    {
        // Damage red alert UI
        if (!redAlertUp)
        {
            StartCoroutine(InitiateRedAlert());
        }

        if (healthProgressBar != null)
        {
            healthProgressBar.value = (float)health / GameManager.Instance.playerMaxHealth;
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
}
