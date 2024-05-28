using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class HUDMenu : Menu
{
    ProgressBar healthProgressBar;

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
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
        if (healthProgressBar != null)
        {
            healthProgressBar.value = (float)health / GameManager.Instance.playerMaxHealth;
        }
    }
}
