using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Notification
{
    public Image fill;
    public Image back;

    public float visibleDuration;

    // Cache game manager's instance to ensure that we don't end up creating it in Ondestroy
    GameManager gameManager = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canvasGroup.alpha = 0;
        gameManager = GameManager.Instance;
        gameManager.OnPlayerStaminaChange += OnPlayerStaminaChange;
        gameManager.OnPlayerUseStamina += OnPlayerUseStamina;
    }

    public void SetFill(float amount)
    {
        fill.fillAmount = amount;
    }

    public void ShowBar(float amount)
    {
        PlayInOut(visibleDuration);
    }

    void OnPlayerStaminaChange(int value)
    {
        SetFill((float)value / gameManager.PlayerMaxStamina);
    }

    void OnPlayerUseStamina(int value)
    {
        ShowBar(value);
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
