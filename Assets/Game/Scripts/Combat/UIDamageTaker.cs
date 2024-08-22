using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageTaker : DamageTaker
{
    [Header("Health Canvas")]
    [SerializeField] protected Slider healthSlider;
    [SerializeField] float canvasTimeout = 10.0f;
    [SerializeField] Canvas healthCanvas;
    CanvasGroup healthCanvasGroup;

    // Flag to detect if object got hit, i.e. canvas should be shown
    bool healthDisplayed = false;

    protected override void Start()
    {
        base.Start();

        // Hide canvas unless required, for efficiency
        if (healthCanvas != null)
        {
            healthCanvasGroup = healthCanvas.gameObject.GetComponent<CanvasGroup>();
            healthCanvas.enabled = false;
        }
    }

    public override bool TakeDamage(Damage damage, bool detectDeath)
    {
        bool damageRegistered = base.TakeDamage(damage, detectDeath);
        if (!damageRegistered)
        {
            return false;
        }

        // Enable health bar canvas
        if (healthSlider != null && healthCanvas != null)
        {
            if (healthCanvas.enabled)
            {
                healthDisplayed = true;
                healthSlider.value = ((float)health / maxHealth);
            }
            else
            {
                healthCanvasGroup.alpha = 1;
                healthCanvas.enabled = true;
                // Update health with a small delay
                StartCoroutine(UpdateHealth(0.1f));
                StartCoroutine(DisableCanvasAfterTimeout());
            }
        }

        return true;
    }

    public override void Death(bool killObject)
    {
        // Hide the UI
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(false);
        }

        base.Death(killObject);
    }

    protected IEnumerator UpdateHealth(float delay)
    {
        yield return new WaitForSeconds(delay);
        healthSlider.value = ((float)health / maxHealth);
    }

    protected IEnumerator DisableCanvasAfterTimeout()
    {
        float timeElapsed = 0.0f;
        while (timeElapsed < canvasTimeout)
        {
            yield return null;

            timeElapsed += Time.deltaTime;
            if (healthDisplayed)
            {
                // Reset time as object got hit again
                timeElapsed = 0.0f;
                healthDisplayed = false;
            }
        }

        // Fade out
        float t = 0.5f;
        while (t >= 0.0f)
        {
            healthCanvasGroup.alpha = t * 2;
            yield return null;
            t -= Time.deltaTime;
        }
        healthCanvas.enabled = false;
    }
}
