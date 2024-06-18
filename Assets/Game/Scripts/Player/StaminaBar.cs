using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : Notification
{
    public Player player;
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
        if (player != null)
        {
            CanvasManager.Instance.AddAnchoredElement(player.transform, rectTransform.GetComponent<RectTransform>(), new Vector2(-70, 100));
        }
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
        SetFill((float)value / GameManager.Instance.PlayerMaxStamina);
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
