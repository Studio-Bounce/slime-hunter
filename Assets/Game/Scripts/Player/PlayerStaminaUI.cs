using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: This class is redundent. Either this or StaminaBar is not required!
// FIX AFTER ALPHA
public class PlayerStaminaUI : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] float staminaTimeout = 3.0f;
    
    bool canvasTriggered = false;  // Used as a flag to detect if canvas should be shown

    Canvas playerCanvas;
    CanvasGroup playerCanvasGroup;

    // Cache game manager's instance to ensure that we don't end up creating it in Ondestroy
    GameManager gameManager = null;

    private void Start()
    {
        // Hide canvas unless required, for efficiency
        playerCanvas = GetComponent<Canvas>();
        playerCanvasGroup = GetComponent<CanvasGroup>();
        playerCanvas.enabled = false;

        gameManager = GameManager.Instance;
        gameManager.OnPlayerStaminaChange += OnPlayerStaminaChange;
        gameManager.OnPlayerUseStamina += OnPlayerUseStamina;
    }

    void OnPlayerStaminaChange(int value)
    {
        SetFill((float)value / gameManager.PlayerMaxStamina);
    }

    public void SetFill(float amount)
    {
        fill.fillAmount = amount;
    }

    void OnPlayerUseStamina(int _)
    {
        // Show the stamina bar
        canvasTriggered = true;
        if (!playerCanvas.enabled)
        {
            playerCanvas.enabled = true;
            playerCanvasGroup.alpha = 1.0f;
            StartCoroutine(DisableCanvasAfterTimeout());
        }
    }

    // TODO: Code duplication with Enemy.DisableCanvasAfterTimeout
    // Fix after alpha build!
    IEnumerator DisableCanvasAfterTimeout()
    {
        float timeElapsed = 0.0f;
        while (timeElapsed < staminaTimeout)
        {
            yield return null;

            timeElapsed += Time.deltaTime;
            if (canvasTriggered)
            {
                // Reset time
                timeElapsed = 0.0f;
                canvasTriggered = false;
            }
        }

        // Fade
        float t = 0.5f;
        while (t >= 0.0f)
        {
            playerCanvasGroup.alpha = t * 2;
            yield return null;
            t -= Time.deltaTime;
        }
        playerCanvas.enabled = false;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerStaminaChange -= OnPlayerStaminaChange;
            gameManager.OnPlayerUseStamina -= OnPlayerUseStamina;
        }
    }
}
